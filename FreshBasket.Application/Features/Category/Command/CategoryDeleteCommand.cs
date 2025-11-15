using FreshBasket.Shared.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshBasket.Application.Features.Category.Command
{
    public class CategoryDeleteCommand : IRequest<ApiResponse<string>>
    {
        public Guid Id { get; set; }
    }
}
