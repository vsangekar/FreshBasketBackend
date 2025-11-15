using FreshBasket.Application.Features.Dashboard.Response;
using FreshBasket.Infrastructure.DbContexts;
using FreshBasket.Shared.Common;
using MediatR;

namespace FreshBasket.Application.Features.Dashboard.Query
{
    public class GetDashboardCountQueryHandler : IRequestHandler<GetDashboardCountQuery, ApiResponse<DashboardCountResponse>>
    {
        private readonly FreshBasketDbContext _context;
        public GetDashboardCountQueryHandler(FreshBasketDbContext context)
        {
            _context = context;
        }
        public Task<ApiResponse<DashboardCountResponse>> Handle(GetDashboardCountQuery request, CancellationToken cancellationToken)
        {
            var totalProducts = _context.Products.Where(x=>x.IsActive==true).Count();
            var totalCategories = _context.Categories.Where(x => x.IsActive == true).Count();
            var totalOrders = _context.Orders.Where(x => x.IsActive == true).Count();
            var totalUsers = _context.Users.Where(x => x.IsActive == true).Count();
            var response = new DashboardCountResponse
            {
                ProductCount  = totalProducts,
                CategoryCount = totalCategories,
                OrderCount    = totalOrders
            };
            return Task.FromResult(new ApiResponse<DashboardCountResponse>
            {
                Success = true,
                Message = "Dashboard Count retrived successfully.",
                Data = response
            });
        }
    }
}
