using FreshBasket.Application.Features.Category.Response;
using FreshBasket.Infrastructure.DbContexts;
using FreshBasket.Shared.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FreshBasket.Application.Features.Category.Query
{
    public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, ApiResponse<List<CategoryResponse>>>
    {
        private readonly FreshBasketDbContext _context;

        public GetCategoryQueryHandler(FreshBasketDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<CategoryResponse>>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            var categories = await _context.Categories
                .Where(x=>x.IsActive==true)
                .Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Image = c.Image,
                    CreatedDate = c.CreatedAt
                })
                .ToListAsync(cancellationToken);

            if (categories == null || !categories.Any())
                return ApiResponse<List<CategoryResponse>>.FailureResponse("No categories found");

            return ApiResponse<List<CategoryResponse>>.SuccessResponse(categories);
        }
    }

}
