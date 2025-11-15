using FreshBasket.Application.Features.Authentication.Response;
using FreshBasket.Shared.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshBasket.Application.Features.Authentication.Command
{
    public class LoginCommand : IRequest<ApiResponse<LoginResponse>>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
