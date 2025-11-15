using FreshBasket.Application.Features.Category.Response;
using FreshBasket.Shared.Common;
using MediatR;

namespace FreshBasket.Application.Features.Category.Query
{
    public class GetCategroyByIdQuery : IRequest<ApiResponse<CategoryResponse>>
    {
        public Guid Id { get; set; }
    }
}
