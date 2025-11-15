using FreshBasket.Application.Features.Dashboard.Response;
using FreshBasket.Shared.Common;
using MediatR;

namespace FreshBasket.Application.Features.Dashboard.Query
{
    public class GetDashboardCountQuery :  IRequest<ApiResponse<DashboardCountResponse>>
    {
    }
}
