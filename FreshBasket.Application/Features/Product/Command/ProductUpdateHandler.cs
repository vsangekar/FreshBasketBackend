using FreshBasket.Infrastructure.DbContexts;
using FreshBasket.Shared.Common;
using MediatR;

namespace FreshBasket.Application.Features.Product.Command
{
    public class ProductUpdateHandler : IRequestHandler<ProductUpdateCommand, ApiResponse<string>>
    {
        private readonly FreshBasketDbContext _context;

        public ProductUpdateHandler(FreshBasketDbContext context)
        {
           _context = context;     
        }
        public Task<ApiResponse<string>> Handle(ProductUpdateCommand request, CancellationToken cancellationToken)
        {
            var product = _context.Products.FirstOrDefault(c => c.Id == request.Id);
            if (product == null)
            {
                return Task.FromResult(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Category not found."
                });
            }
            product.Description = request.Description;
            product.Name = request.Name;
            product.CategoryId =request.CategoryId;
            product.StockQuantity = request.StockQuantity;
            product.Image = request.ImageUrl;
            product.UpdatedAt =DateTime.Now;
            _context.Products.Update(product);
            _context.SaveChanges();
            return Task.FromResult(new ApiResponse<string>
            {
                Success = true,
                Message = "product updated successfully."
            });
        }
    }
}
