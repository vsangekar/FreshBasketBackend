using FreshBasket.Application.Features.Authentication.Response;
using FreshBasket.Infrastructure;
using FreshBasket.Infrastructure.DbContexts;
using FreshBasket.Infrastructure.Entities.Authentication;
using FreshBasket.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FreshBasket.Application.Features.Authentication.Command
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResponse>>
    {
        private readonly IConfiguration _configuration;
        private readonly FreshBasketDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public LoginCommandHandler(IConfiguration configuration, FreshBasketDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<ApiResponse<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null)
                return ApiResponse<LoginResponse>.FailureResponse("Invalid email or password");

            bool isValid = _passwordHasher.VerifyPassword(
                request.Password,
                user.PasswordHash,
                user.PasswordSalt
            );

            if (!isValid)
                return ApiResponse<LoginResponse>.FailureResponse("Invalid email or password");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                   new Claim(ClaimTypes.Name, user.UserName),
                   new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.Name)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            string jwtToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

            var result = new LoginResponse
            {
                Token = jwtToken,
                Role = user.Role.Name,
                Message = "Login successful"
            };

            return ApiResponse<LoginResponse>.SuccessResponse(result, "Login successful");
        }



    }
}
