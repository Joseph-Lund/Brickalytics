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
        public async Task<Result<ProductSoldParent>> GetProductsSold(Dates dates)
        {
            dates.Start = new DateTime(dates.Start.Year, dates.Start.Month, dates.Start.Day, 0, 0, 0);
            dates.End = new DateTime(dates.End.Year, dates.End.Month, dates.End.Day, 23, 59, 59);
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            var user = await _userService.GetUserByIdAsync(_tokenHelper.GetUserId(accessToken));
            if (user == null)
            {
                throw new Exception();
            }
            try
            {
                var data = await GetProductsSold(user, dates.Start, dates.End);
                return new Result<ProductSoldParent> { Code = 200, Message = "Success", Data = data };
            }
            catch (Exception)
            {
                return new Result<ProductSoldParent> { Code = 500, Message = "Could not get products sold" };
            }
        }
        [HttpPost]
        [Route("ProductsSoldAdmin")]
        public async Task<Result<ProductSoldParent>> GetProductsSoldAdmin(Dates dates)
        {
            dates.Start = new DateTime(dates.Start.Year, dates.Start.Month, dates.Start.Day, 0, 0, 0);
            dates.End = new DateTime(dates.End.Year, dates.End.Month, dates.End.Day, 23, 59, 59);
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            var admin = await _userService.GetUserByIdAsync(_tokenHelper.GetUserId(accessToken));
            var user = await _userService.GetUserByIdAsync(Convert.ToInt32(dates.Id));
            if (user == null || admin.IsAdmin != true)
            {
                throw new Exception();
            }

            try
            {
                var data = await GetProductsSold(user, dates.Start, dates.End);
                return new Result<ProductSoldParent> { Code = 200, Message = "Success", Data = data };
            }
            catch (Exception)
            {
                return new Result<ProductSoldParent> { Code = 500, Message = "Could not get products sold" };
            }
        }
        [HttpGet]
        [Route("Payment/{userId:int}")]
        public async Task<Result<List<Payment>>> GetPayments(int userId)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            var admin = await _userService.GetUserByIdAsync(_tokenHelper.GetUserId(accessToken));
            var user = await _userService.GetUserByIdAsync(userId);
            if (admin == null)
            {
                throw new Exception();
            }
            try
            {
                if (admin.IsAdmin != true)
                {
                    var data = await GetPaymentsCalculations(admin);
                    return new Result<List<Payment>> { Code = 200, Message = "Success", Data = data };
                }
                var adminData = await GetPaymentsCalculations(user);
                return new Result<List<Payment>> { Code = 200, Message = "Success", Data = adminData };
            }
            catch (Exception)
            {
                return new Result<List<Payment>> { Code = 500, Message = "Could not get payments" };
            }
        }
        [HttpPost]
        [Route("Payment")]
        public async Task<Result<int>> AddPayment(Payment payment)
        {

            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            var admin = await _userService.GetUserByIdAsync(_tokenHelper.GetUserId(accessToken));
            var user = await _userService.GetUserByIdAsync(Convert.ToInt32(payment.UserId));
            if (user == null || admin.IsAdmin != true)
            {
                throw new Exception();
            }
            try
            {
                var data = await _userService.AddUserPaymentAsync(payment);
                return new Result<int> { Code = 200, Message = "Success", Data = data };
            }
            catch (Exception)
            {
                return new Result<int> { Code = 500, Message = "Could not add payment"};
            }
        }

        private async Task<ProductSoldParent> GetProductsSold(User user, DateTime startDate, DateTime endDate)
        {
            var productsSoldTotal = 0;
            var productsSoldProfit = (decimal)0.0;

            var rates = await _userService.GetUserRatesAsync(user);
            var orders = await _shopifyService.GetCreatorsAnalyticsAsync(user, rates, startDate, endDate);
            List<ProductSoldChild> items = new List<ProductSoldChild>();

            foreach (var order in orders)
            {
                if (order.Count > 0)
                {
                    foreach (var rate in rates)
                    {
                        if ((int)order.ProductType == rate.ProductTypeId)
                        {
                            decimal total = (decimal)0.0;
                            if (rate.Rate != null)
                            {
                                total = order.Count * Convert.ToDecimal(rate.Rate);
                            }
                            else
                            {

                                total = order.Count * (order.Price * Convert.ToDecimal(rate.Percent));
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

        private async Task<List<Payment>> GetPaymentsCalculations(User user)
        {
            var lastPayment = await _userService.GetLastPaymentAsync(user.Id);
            var orderProfit = Math.Round((await GetProductsSold(user, lastPayment, DateTime.Now)).ProductsSoldProfit, 2);

            var response = await _userService.GetUserPaymentsAsync(user.Id);
            response = response.Where(payment => payment.Id > 6).ToList();
            response.Insert(0, new Payment() { Id = 0, UserId = null, PaymentAmount = orderProfit, PaymentDate = DateTime.Now });
            return response;
        }
    }
}
