using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshBasket.Application.Features.Authentication.Command
{
    public class AdminRegisterCommand : IRequest<string>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
