using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Brickalytics.Services;
using Brickalytics.Models;

namespace Brickalytics.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProductSubTypeController : ControllerBase
    {
        private readonly ILogger<ProductSubTypeController> _logger;
        private readonly IProductSubTypeService _productSubTypeService;

        public ProductSubTypeController(ILogger<ProductSubTypeController> logger, IProductSubTypeService productSubTypeService)
        {
            _logger = logger;
            _productSubTypeService = productSubTypeService;
        }

        [HttpGet]
        public async Task<List<ProductSubType>> GetProductSubTypes()
        {
            var result = await _productSubTypeService.GetProductSubTypesAsync();
            return result;
        }
    }
}
