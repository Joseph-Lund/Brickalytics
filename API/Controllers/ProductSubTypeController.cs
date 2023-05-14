using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Brickalytics.Services;
using Brickalytics.Models;
using Brickalytics.Helpers;
using Microsoft.Net.Http.Headers;

namespace Brickalytics.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProductSubTypeController : ControllerBase
    {
        private readonly ILogger<ProductSubTypeController> _logger;
        private readonly IProductSubTypeService _productSubTypeService;
        private readonly ITokenHelper _tokenHelper;
        private readonly string _accessToken;

        public ProductSubTypeController(ILogger<ProductSubTypeController> logger, IProductSubTypeService productSubTypeService, ITokenHelper tokenHelper)
        {
            _logger = logger;
            _productSubTypeService = productSubTypeService;
            _tokenHelper = tokenHelper;
            _accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
        }

        [HttpGet]
        public async Task<List<ProductSubType>> GetProductSubTypes()
        {
            if(_tokenHelper.IsUserAdmin(_accessToken))
            {
                var result = await _productSubTypeService.GetProductSubTypesAsync();
                return result;
            }
            throw new UnauthorizedAccessException();
        }
    }
}
