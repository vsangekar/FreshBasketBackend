using FreshBasket.Infrastructure.DbContexts;
using FreshBasket.Shared.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace FreshBasket.Application.Features.Category.Command
{
    public class CategoryCommandHandler : IRequestHandler<CategoryCommand, ApiResponse<string>>
    {
        private readonly FreshBasketDbContext _context;
        public CategoryCommandHandler(FreshBasketDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> Handle(CategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var category = new Infrastructure.Entities.Category
                {
                    Id          = new Guid(),
                    Name        = request.Name,
                    Description = request.Description,
                    Image       = request.ImageUrl,
                    CreatedAt   = DateTime.UtcNow,
                    UpdatedAt   = DateTime.UtcNow,
                    IsActive    = true,
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync(cancellationToken);

                return new ApiResponse<string>
                {
                    Data = "Category added successfully",
                    Success = true,
                    Message = "Category added successfully"
                };
            }
            catch (DbUpdateException dbEx)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Database update failed: {dbEx.InnerException?.Message ?? dbEx.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = $"An unexpected error occurred: {ex.Message}"
                };
            }
        }

    }
}
