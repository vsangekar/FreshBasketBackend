using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshBasket.Application.Features.Dashboard.Response
{
    public class DashboardCountResponse
    {
        public int ProductCount { get; set; }
        public int CategoryCount { get; set; }
        public int UserCount { get; set; }
        public int OrderCount { get; set; }
    }
}
