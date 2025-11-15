using FreshBasket.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Http;


namespace FreshBasket.Application.Features.Category.Command
{
    public class CategoryCommand : IRequest<ApiResponse<string>>
    {
        public Guid Id { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
