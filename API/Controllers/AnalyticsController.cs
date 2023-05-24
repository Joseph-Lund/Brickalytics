using Brickalytics.Helpers;
using Brickalytics.Models;
using Brickalytics.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

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
        private readonly ITokenHelper _tokenHelper;

        public AnalyticsController(ILogger<AnalyticsController> logger, IUserService userService, IShopifyService shopifyService, ITokenHelper tokenHelper)
        {
            _logger = logger;
            _shopifyService = shopifyService;
            _userService = userService;
            _tokenHelper = tokenHelper;
        }

        [HttpPost]
        [Route("ProductsSold")]
        public async Task<ProductSoldParent> GetProductsSold(Dates dates)
        {
            dates.Start = new DateTime(dates.Start.Year, dates.Start.Month, dates.Start.Day, 0, 0, 0);
            dates.End = new DateTime(dates.End.Year, dates.End.Month, dates.End.Day, 23, 59, 59);
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            var user = await _userService.GetUserByIdAsync(_tokenHelper.GetUserId(accessToken));
            if (user == null)
            {
                throw new Exception();
            }
            var productsSoldTotal = 0;
            var productsSoldProfit = (decimal)0.0;

            var rates = await _userService.GetUserRatesAsync(user);
            var orders = await _shopifyService.GetCreatorsAnalyticsAsync(user, rates, dates.Start, dates.End);
            List<ProductSoldChild> items = new List<ProductSoldChild>();

            foreach (var order in orders)
            {
                if (order.Count > 0)
                {
                    foreach (var rate in rates)
                    {
                        if ((int)order.ProductType == rate.ProductTypeId)
                        {
                            decimal? total;
                            if (rate.Rate != null)
                            {
                                total = (order.Count * rate.Rate);
                            }
                            else
                            {

                                total = (order.Count * order.Price) * rate.Percent;
                            }
                            productsSoldTotal += order.Count;
                            productsSoldProfit += (decimal)total;
                            var item = new ProductSoldChild()
                            {
                                Count = order.Count,
                                ItemName = order.Name,
                                Total = (decimal)total
                            };
                            items.Add(item);
                        }
                    }
                }
            }

            var model = new ProductSoldParent()
            {
                ProductsSoldProfit = productsSoldProfit,
                ProductsSoldTotal = productsSoldTotal,
                Items = items
            };
            return model;
        }
        [HttpPost]
        [Route("ProductsSoldAdmin")]
        public async Task<ProductSoldParent> GetProductsSoldAdmin(Dates dates)
        {
            dates.Start = new DateTime(dates.Start.Year, dates.Start.Month, dates.Start.Day, 0, 0, 0);
            dates.End = new DateTime(dates.End.Year, dates.End.Month, dates.End.Day, 23, 59, 59);
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            var admin = await _userService.GetUserByIdAsync(_tokenHelper.GetUserId(accessToken));
            var user = await _userService.GetUserByIdAsync((int)dates.Id);
            if (user == null || admin.IsAdmin != true)
            {
                throw new Exception();
            }
            var productsSoldTotal = 0;
            var productsSoldProfit = (decimal)0.0;

            var rates = await _userService.GetUserRatesAsync(user);
            var orders = await _shopifyService.GetCreatorsAnalyticsAsync(user, rates, dates.Start, dates.End);
            List<ProductSoldChild> items = new List<ProductSoldChild>();

            foreach (var order in orders)
            {
                if (order.Count > 0)
                {
                    foreach (var rate in rates)
                    {
                        if ((int)order.ProductType == rate.ProductTypeId)
                        {
                            decimal? total;
                            if (rate.Rate != null)
                            {
                                total = (order.Count * rate.Rate);
                            }
                            else
                            {

                                total = (order.Count * order.Price) * rate.Percent;
                            }
                            productsSoldTotal += order.Count;
                            productsSoldProfit += (decimal)total;
                            var item = new ProductSoldChild()
                            {
                                Count = order.Count,
                                ItemName = order.Name,
                                Total = (decimal)total
                            };
                            items.Add(item);
                        }
                    }
                }
            }

            var model = new ProductSoldParent()
            {
                ProductsSoldProfit = productsSoldProfit,
                ProductsSoldTotal = productsSoldTotal,
                Items = items
            };
            return model;
        }
        [HttpGet]
        [Route("Payment/{userId:int}")]
        public async Task<List<Payment>> GetPayments(int userId)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            var admin = await _userService.GetUserByIdAsync(_tokenHelper.GetUserId(accessToken));
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null || admin.IsAdmin != true)
            {
                throw new Exception();
            }
            if (admin.IsAdmin != true)
            {
                return await _userService.GetUserPaymentsAsync(admin.Id);
            }

            return await _userService.GetUserPaymentsAsync(userId);
        }
        [HttpPost]
        [Route("Payment")]
        public async Task<int> AddPayment(Payment payment)
        {

            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            var admin = await _userService.GetUserByIdAsync(_tokenHelper.GetUserId(accessToken));
            var user = await _userService.GetUserByIdAsync((int)payment.UserId);
            if (user == null || admin.IsAdmin != true)
            {
                throw new Exception();
            }

            var payments = await _userService.AddUserPaymentAsync(payment);
            return payments;
        }
    }
}
