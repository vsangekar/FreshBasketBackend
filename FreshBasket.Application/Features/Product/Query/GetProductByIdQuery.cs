using FreshBasket.Application.Features.Product.Response;
using FreshBasket.Shared.Common;
using MediatR;

namespace FreshBasket.Application.Features.Product.Query
{
    public class GetProductByIdQuery : IRequest<ApiResponse<ProductResponse>>
    {
        public Guid Id { get; set; }
    }
}
