using FreshBasket.Infrastructure.DbContexts;
using FreshBasket.Infrastructure.Entities.Authentication;
using FreshBasket.Shared.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FreshBasket.Application.Features.Authentication.Command
{
    public class AdminRegisterCommandHandler : IRequestHandler<AdminRegisterCommand, string>
    {
        private readonly FreshBasketDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;
        public AdminRegisterCommandHandler(FreshBasketDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }
        public async Task<string> Handle(AdminRegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

                if (existingUser != null)
                    return "Admin already exists with this email.";

                var adminRole = await _dbContext.Roles
                    .FirstOrDefaultAsync(r => r.Name == "Admin", cancellationToken);

                if (adminRole == null)
                    return "Admin role not found.";

                var (hash, salt) = _passwordHasher.HashPassword(request.Password);

                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    UserName =  request.Email,
                    Email = request.Email,
                    PasswordHash = hash,        
                    PasswordSalt = salt,        
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    RoleId = adminRole.Id
                };

                _dbContext.Users.Add(adminUser);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return "Admin registered successfully.";
            }
            catch (DbUpdateException)
            {
                return "Database error during registration.";
            }
            catch (Exception)
            {
                return "Unexpected error.";
            }
        }


    }
}
