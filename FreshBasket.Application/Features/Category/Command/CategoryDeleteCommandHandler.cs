using FreshBasket.Application.Features.Category.Query;
using FreshBasket.Application.Features.Category.Response;
using FreshBasket.Infrastructure.DbContexts;
using FreshBasket.Shared.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshBasket.Application.Features.Category.Command
{
    public class CategoryDeleteCommandHandler : IRequestHandler<CategoryDeleteCommand, ApiResponse<string>>
    {
        private readonly FreshBasketDbContext _context;
        public CategoryDeleteCommandHandler(FreshBasketDbContext context)
        {
            _context = context;
        }
        public Task<ApiResponse<string>> Handle(CategoryDeleteCommand request, CancellationToken cancellationToken)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == request.Id); 
            if (category == null)
            {
                return Task.FromResult(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Category not found."
                });
            }
            category.IsActive = false;
            _context.Update(category);
            _context.SaveChanges();
            return Task.FromResult(new ApiResponse<string>
            {
                Success = true,
                Message = "Category deleted successfully."
            });
        }

    }
}
