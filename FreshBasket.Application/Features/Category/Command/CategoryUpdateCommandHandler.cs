using FreshBasket.Infrastructure.DbContexts;
using FreshBasket.Shared.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshBasket.Application.Features.Category.Command
{
    public class CategoryUpdateCommandHandler : IRequestHandler<CategoryUpdateCommand, ApiResponse<string>>
    {
        private readonly FreshBasketDbContext _context;
        public CategoryUpdateCommandHandler(FreshBasketDbContext context)
        {
            _context = context;
        }
        public Task<ApiResponse<string>> Handle(CategoryUpdateCommand request, CancellationToken cancellationToken)
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
            category.Description = request.Description;
            category.Name = request.Name;
            category.Image = request.ImageUrl;
            _context.Categories.Update(category);
            _context.SaveChanges();
            return Task.FromResult(new ApiResponse<string>
            {
                Success = true,
                Message = "Category updated successfully."
            });
        }
    }
}
