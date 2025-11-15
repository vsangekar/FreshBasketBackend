using FreshBasket.Application.Features.Category.Query;
using FreshBasket.Application.Features.Category.Response;
using FreshBasket.Application.Features.Product.Response;
using FreshBasket.Infrastructure.DbContexts;
using FreshBasket.Shared.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FreshBasket.Application.Features.Product.Query
{
    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ApiResponse<List<ProductResponse>>>
    {
        private readonly FreshBasketDbContext _context;
        public GetProductQueryHandler(FreshBasketDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse<List<ProductResponse>>> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.Where(x => x.IsActive == true)
                .Select(c => new ProductResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Price = c.Price,
                    StockQuantithy = c.StockQuantity,
                    CategoryId = c.CategoryId,
                    ImageUrl =c.Image,
                })
                .ToListAsync(cancellationToken);

            if (product == null || !product.Any())
                return ApiResponse<List<ProductResponse>>.FailureResponse("No categories found");

            return ApiResponse<List<ProductResponse>>.SuccessResponse(product);
        }
    }
}
