using FreshBasket.Application.Features.Product.Response;
using FreshBasket.Infrastructure.DbContexts;
using FreshBasket.Shared.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FreshBasket.Application.Features.Product.Query
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ApiResponse<ProductResponse>>
    {
        private readonly FreshBasketDbContext _context;
        public GetProductByIdQueryHandler(FreshBasketDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
               .AsNoTracking()
               .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (product == null)
            {
                return ApiResponse<ProductResponse>.FailureResponse("Product not found");
            }

            var response = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId =product.CategoryId,
                StockQuantithy =product.StockQuantity,
                ImageUrl = product.Image
            };
            return ApiResponse<ProductResponse>.SuccessResponse(response, "Product fetched successfully");
        }
    }
}
