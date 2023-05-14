using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Brickalytics.Models;
using Brickalytics.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Brickalytics.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly ILogger<AnalyticsController> _logger;
        private readonly IShopifyService _shopifyService;
        private readonly IUserService _userService;

        public AnalyticsController(ILogger<AnalyticsController> logger, IUserService userService, IShopifyService shopifyService)
        {
            _logger = logger;
            _shopifyService = shopifyService;
            _userService = userService;
        }

        [HttpGet]
        [Route("ProductsSold/{userId:int}")]
        public async Task<ProductSoldParent> GetProductsSold(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new Exception();
            }
            var productsSoldTotal = 0;
            var orders = await _shopifyService.GetCreatorsAnalytics(user);
            var rates = await _userService.GetUserRatesByIdAsync(user.Id);
            List<ProductSoldChild> items = new List<ProductSoldChild>();

            foreach(var order in orders)
            {
                productsSoldTotal += order.Count;
                var total = order.Count * order.Price;
                var item = new ProductSoldChild()
                {
                    Count = order.Count, 
                    ItemName = order.Name, 
                    Price = order.Price,
                    Total = total
                };
                items.Add(item);
            }

            var model = new ProductSoldParent()
            {
                ProductsSoldTotal = productsSoldTotal,
                Items = items
            };
            return model;
        }
        // [HttpGet]
        // [Route("Details/{userId:int}")]
        // public async Task<ProductSoldDetail> GetDetails(int userId)
        // {
        //     var user = await _userService.GetUserByIdAsync(userId);
        //     if (user == null)
        //     {
        //         throw new Exception();
        //     }
        //     var productsSoldTotal = 0;
        //     var orders = await _shopifyService.GetCreatorsAnalytics(user);
        //     var rates = await _userService.GetUserRatesByIdAsync(user.Id);
        //     // List<ProductSoldChild> items = new List<ProductSoldChild>();

        //     foreach(var order in orders)
        //     {
        //         productsSoldTotal += order.Count;
        //         var total = order.Count * order.Price;
        //         var item = new ProductSoldChild()
        //         {
        //             Count = order.Count, 
        //             ItemName = order.Name, 
        //             Price = order.Price,
        //             Total = total
        //         };
        //         items.Add(item);
        //     }

        //     // var model = new ProductSoldParent()
        //     // {
        //     //     ProductsSoldTotal = productsSoldTotal,
        //     //     Items = items
        //     // };
        //     return model;
        // }
    }
}
