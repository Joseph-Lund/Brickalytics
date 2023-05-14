using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Brickalytics.Services;
using Brickalytics.Models;
using Microsoft.Net.Http.Headers;
using Brickalytics.Helpers;

namespace Brickalytics.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProductTypeController : ControllerBase
    {
        private readonly ILogger<ProductTypeController> _logger;
        private readonly IProductTypeService _productTypeService;
        private readonly ITokenHelper _tokenHelper;
        private readonly string _accessToken;

        public ProductTypeController(ILogger<ProductTypeController> logger, IProductTypeService productTypeService, ITokenHelper tokenHelper)
        {
            _logger = logger;
            _productTypeService = productTypeService;
            _tokenHelper = tokenHelper;
            _accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
        }

        [HttpGet]
        public async Task<List<ProductType>> GetProductTypes()
        {
            if(_tokenHelper.IsUserAdmin(_accessToken))
            {
                var result = await _productTypeService.GetProductTypesAsync();
                return result;
            }
            throw new UnauthorizedAccessException();
        }
    }
}
