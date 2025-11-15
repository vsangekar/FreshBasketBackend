using FreshBasket.Application.Features.Authentication.Response;
using FreshBasket.Shared.Common;
using MediatR;

namespace FreshBasket.Application.Features.Product.Command
{
    public class ProductCommand : IRequest<ApiResponse<string>>
    {
        public required string Name{ get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public string? ImageUrl { get; set; }
    }
}
