using FreshBasket.Shared.Common;
using MediatR;

namespace FreshBasket.Application.Features.Product.Command
{
    public class ProductUpdateCommand : IRequest<ApiResponse<string>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public string ImageUrl { get; set; }
    }
}
