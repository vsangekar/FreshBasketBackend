using FreshBasket.Shared.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshBasket.Application.Features.Product.Command
{
    public class ProductDeleteCommand : IRequest<ApiResponse<string>>
    {
        public Guid Id { get; set; }
    }
}
