using FreshBasket.Application.Features.Category.Response;
using FreshBasket.Infrastructure.DbContexts;
using FreshBasket.Shared.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FreshBasket.Application.Features.Category.Query
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategroyByIdQuery, ApiResponse<CategoryResponse>>
    {
        private readonly FreshBasketDbContext _context;

        public GetCategoryByIdQueryHandler(FreshBasketDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<CategoryResponse>> Handle(GetCategroyByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (category == null)
            {
                return ApiResponse<CategoryResponse>.FailureResponse("Category not found");
            }

            var response = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Image = category.Image
            };
            return ApiResponse<CategoryResponse>.SuccessResponse(response, "Category fetched successfully");
        }
    }
}