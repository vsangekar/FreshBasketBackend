using FreshBasket.Infrastructure.DbContexts;
using FreshBasket.Shared.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace FreshBasket.Application.Features.Product.Command
{
    public class ProductCommandHandler : IRequestHandler<ProductCommand, ApiResponse<string>>
    {
        private readonly FreshBasketDbContext _context;

        public ProductCommandHandler(FreshBasketDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse<string>> Handle(ProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product= new Infrastructure.Entities.Product
                { 
                    Id = new Guid(),
                    Name = request.Name,
                    Description = request.Description,
                    Price=request.Price,
                    Image = request.ImageUrl,
                    StockQuantity=request.StockQuantity,
                    CategoryId=request.CategoryId,
                    CreatedAt =DateTime.Now,
                    UpdatedAt =DateTime.Now,
                    IsActive =true,
                };
                _context.Products.Add(product);
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
