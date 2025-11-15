using FreshBasket.Application.Features.Category.Command;
using FreshBasket.Application.Features.Category.Query;
using FreshBasket.Application.Features.Category.Response;
using FreshBasket.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FreshBasket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _memoryCache;

        public CategoryController(IMediator mediator, IWebHostEnvironment env, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _env = env;
            _memoryCache = memoryCache;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddCategory([FromForm] CategoryCommand command, IFormFile file)
        {
            try
            {
                string imagePath = null;

                // File upload logic
                if (file != null && file.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/category");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    imagePath = $"/uploads/category/{uniqueFileName}";
                }

                var addCommand = new CategoryCommand
                {
                    Name = command.Name,
                    Description = command.Description,
                    ImageUrl = imagePath
                };

                var response = await _mediator.Send(addCommand);

                _memoryCache.Remove("Categories");

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
        public async Task<IActionResult> UpdateCategory([FromForm] string commandJson, IFormFile? file)
        {
            try
            {
                if (string.IsNullOrEmpty(commandJson))
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Command data is required" });

                var command = System.Text.Json.JsonSerializer.Deserialize<CategoryUpdateCommand>(
                    commandJson,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (command == null)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Invalid command data" });

                string imagePath = command.ImageUrl;

                // File update
                if (file != null && file.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/category");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    imagePath = $"/uploads/category/{uniqueFileName}";
                    command.ImageUrl = imagePath;
                }

                var response = await _mediator.Send(command);

                // Cache invalidate
                _memoryCache.Remove("Categories");
                _memoryCache.Remove($"Category_{command.Id}");

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

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteCategory([FromBody] CategoryDeleteCommand command)
        {
            var result = await _mediator.Send(command);

            // Cache invalidate
            _memoryCache.Remove("Categories");
            _memoryCache.Remove($"Category_{command.Id}");

            return Ok(new ApiResponse<string>
            {
                Success = result.Success,
                Message = result.Success ? "Category deleted successfully" : "Failed to delete category"
            });
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetCategory([FromQuery] GetCategoryQuery command)
        {
            try
            {
                if (!_memoryCache.TryGetValue("Categories", out ApiResponse<List<CategoryResponse>> cachedCategories))
                {
                    var result = await _mediator.Send(command);

                    if (result.Success && result.Data != null)
                    {
                        var cacheOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                            SlidingExpiration = TimeSpan.FromMinutes(2)
                        };

                        _memoryCache.Set("Categories", result, cacheOptions);
                    }

                    return Ok(result);
                }

                return Ok(cachedCategories);
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

        [HttpGet("getById")]
        public async Task<IActionResult> GetCategoryById([FromQuery] GetCategroyByIdQuery command)
        {
            try
            {
                string cacheKey = $"Category_{command.Id}";

                if (!_memoryCache.TryGetValue(cacheKey, out ApiResponse<object>? cachedCategory))
                {
                    var result = await _mediator.Send(command);

                    if (result.Success && result.Data != null)
                    {
                        var cacheOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                        };

                        // Save to cache
                        _memoryCache.Set(cacheKey, result, cacheOptions);
                    }

                    return Ok(result);
                }

                // Return from cache
                return Ok(cachedCategory);
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
    }
}
