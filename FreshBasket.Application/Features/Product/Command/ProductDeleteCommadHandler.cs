using FreshBasket.Application.Features.Category.Command;
using FreshBasket.Infrastructure.DbContexts;
using FreshBasket.Shared.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshBasket.Application.Features.Product.Command
{
    public class ProductDeleteCommadHandler : IRequestHandler<ProductDeleteCommand, ApiResponse<string>>
    {
        private readonly FreshBasketDbContext _context;
        public ProductDeleteCommadHandler(FreshBasketDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse<string>> Handle(ProductDeleteCommand request, CancellationToken cancellationToken)
        {
            var product = _context.Products.FirstOrDefault(c => c.Id == request.Id);
            if (product == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Product not found"
                };
            }
            product.IsActive = false;
            _context.Update(product);
            await _context.SaveChangesAsync();
            return new ApiResponse<string>
            {
                Success = true,
                Message = "Product deleted successfully"
            };
        }
    }
}
