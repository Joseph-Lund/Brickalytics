using Brickalytics.Services;
using Moq;
using Xunit;

namespace BrickalyticsTests.Tests
{
    public class ShopifyTests
    {
        private ShopifyService _shopifyService;

        public ShopifyTests()
        {
            var mock = new Mock<ILogger<ShopifyService>>();
            ILogger<ShopifyService> logger = mock.Object;

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("secrets.json")
            .Build();

            _shopifyService = new ShopifyService(logger, configuration);
        }

        [Fact]
        public async Task GetOrders()
        {
            var orders = await _shopifyService.GetOrdersAsync();
            Assert.NotNull(orders);
        }
        [Fact]
        public async Task GetProducts()
        {
            var products = await _shopifyService.GetProductsAsync();
            Assert.NotNull(products);
        }
        [Fact]
        public async Task GetCollections()
        {
            var collections = await _shopifyService.GetCollectionsAsync();
            Assert.NotNull(collections);
        }
        [Fact]
        public async Task GetCollectionsProductsAsync()
        {
            long id = 266502930541;
            var collectionsProducts = await _shopifyService.GetCollectionsProductsAsync(id);
            Assert.NotNull(collectionsProducts);
        }
        [Fact]
        public async Task GetCreatorsAnalytics()
        {
            long id = 266502930541;
            var collectionsProducts = await _shopifyService.GetCollectionsProductsAsync(id);
            Assert.NotNull(collectionsProducts);
        }
        
        // [Fact]
        // public async Task GetProductsSoldCount()
        // {
        //     IList<long?> productIds = new List<long?>()
        //     {
        //         8319259869499,
        //         8276458242363
        //     };

        //     var productsSoldCount = await _shopifyService.GetProductsSoldCountAsync(productIds);
        //     Assert.NotNull(productsSoldCount);
        // }
    }
}