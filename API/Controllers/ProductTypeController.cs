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

        public ProductTypeController(ILogger<ProductTypeController> logger, IProductTypeService productTypeService, ITokenHelper tokenHelper)
        {
            _logger = logger;
            _productTypeService = productTypeService;
            _tokenHelper = tokenHelper;
        }

        [HttpGet]
        public async Task<Result<List<ProductType>>> GetProductTypes()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            try
            {
                if (_tokenHelper.IsUserAdmin(accessToken))
                {
                    var data = await _productTypeService.GetProductTypesAsync();
                    return new Result<List<ProductType>>() { Code = 200, Message = "Success", Data = data };
                }
                else
                {
                    throw new UnauthorizedAccessException("Not authorized to get Product Types");
                }
            }
            catch (Exception ex)
            {

                return new Result<List<ProductType>>() { Code = 500, Message = ex.Message };
            }
        }
    }
}
