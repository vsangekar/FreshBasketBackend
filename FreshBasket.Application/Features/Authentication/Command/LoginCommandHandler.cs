using FreshBasket.Application.Features.Authentication.Response;
using FreshBasket.Infrastructure;
using FreshBasket.Infrastructure.DbContexts;
using FreshBasket.Infrastructure.Entities.Authentication;
using FreshBasket.Shared.Common;
using MediatR;
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

        public LoginCommandHandler(IConfiguration configuration, FreshBasketDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            // Generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            var responseData = new LoginResponse
            {
                Token = jwtToken,
                Role = user.Role.Name.ToString(),
                Message = "Login successful."
            };
            return ApiResponse<LoginResponse>.SuccessResponse(responseData, "Login successful.");
        }
    }
}
