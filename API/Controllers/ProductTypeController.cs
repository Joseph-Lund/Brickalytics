using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Brickalytics.Services;
using Brickalytics.Models;

namespace Brickalytics.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProductTypeController : ControllerBase
    {
        private readonly ILogger<ProductTypeController> _logger;
        private readonly IProductTypeService _productTypeService;

        public ProductTypeController(ILogger<ProductTypeController> logger, IProductTypeService productTypeService)
        {
            _logger = logger;
            _productTypeService = productTypeService;
        }

        [HttpGet]
        public async Task<List<ProductType>> GetProductTypes()
        {
            var result = await _productTypeService.GetProductTypesAsync();
            return result;
        }
    }
}
