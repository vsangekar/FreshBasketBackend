using FreshBasket.Application.Features.Product.Response;
using FreshBasket.Shared.Common;
using MediatR;

namespace FreshBasket.Application.Features.Product.Query
{
    public class GetProductQuery : IRequest<ApiResponse<List<ProductResponse>>>
    {

    }
}
