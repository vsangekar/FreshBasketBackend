using FreshBasket.Application.Features.Category.Response;
using FreshBasket.Shared.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshBasket.Application.Features.Category.Query
{
    public class GetCategoryQuery : IRequest<ApiResponse<List<CategoryResponse>>>
    {

    }
}
