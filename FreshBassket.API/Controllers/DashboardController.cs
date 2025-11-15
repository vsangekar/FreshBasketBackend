using FreshBasket.Application.Features.Dashboard.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FreshBasket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _memoryCache;

        public DashboardController(IMediator mediator, IWebHostEnvironment env, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _env = env;
            _memoryCache = memoryCache;
        }

        [HttpGet("getDashboardCount")]
        public async Task<IActionResult> GetProductCount([FromQuery] GetDashboardCountQuery query)
        {
            const string cacheKey = "dashboard_count";

            if (!_memoryCache.TryGetValue(cacheKey, out object? cachedResult))
            {
                var result = await _mediator.Send(query);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                _memoryCache.Set(cacheKey, result, cacheOptions);
                cachedResult = result;
            }

            return Ok(new { Success = true, Data = cachedResult });
        }
    }
}
