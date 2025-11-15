using FreshBasket.Application.Features.Product.Command;
using FreshBasket.Application.Features.Product.Query;
using FreshBasket.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FreshBasket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _memoryCache;

        public ProductController(IMediator mediator, IWebHostEnvironment env, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _env = env;
            _memoryCache = memoryCache;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromForm] ProductCommand command, IFormFile? file)
        {
            try
            {
                string imagePath = null;

                // Save Image
                if (file != null && file.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "products");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    imagePath = $"/uploads/products/{uniqueFileName}";
                }

                command.ImageUrl = imagePath;

                var response = await _mediator.Send(command);

                // Clear cache after adding
                if (response.Success)
                    _memoryCache.Remove("all_products");

                return response.Success ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductUpdateCommand command,IFormFile? file)  
        {
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "products");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                command.ImageUrl = $"/uploads/products/{uniqueFileName}";

                Console.WriteLine($"✅ New image uploaded: {command.ImageUrl}");
            }
            else
            {
                Console.WriteLine($"ℹ️ No new image uploaded, keeping existing: {command.ImageUrl}");
            }

            var response = await _mediator.Send(command);
            if (response.Success)
                _memoryCache.Remove("all_products");

            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteProduct([FromBody] ProductDeleteCommand command)
        {
            var result = await _mediator.Send(command);

            _memoryCache.Remove("all_products");

            return Ok(new { Success = result });
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetAllProducts()
        {
            const string cacheKey = "all_products";

            if (!_memoryCache.TryGetValue(cacheKey, out object? cachedProducts))
            {
                var result = await _mediator.Send(new GetProductQuery());

                if (result != null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                    _memoryCache.Set(cacheKey, result, cacheOptions);
                    cachedProducts = result;
                }
            }

            return Ok(cachedProducts);
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery { Id = id });
            return Ok(result);
        }
    }
}
