using Brickalytics.Models;

namespace Brickalytics.Services
{
    public class ShopifyService : IShopifyService
    {
        private readonly ILogger<ShopifyService> _logger;
        private readonly IConfiguration _config;

        public ShopifyService(ILogger<ShopifyService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        public async Task<IEnumerable<ShopifySharp.Order>> GetOrdersAsync()
        {
            var orderService = CreateService<ShopifySharp.OrderService>();
            IEnumerable<ShopifySharp.Order> orders = (await orderService.ListAsync()).Items;
            return orders;
        }
        public async Task<IEnumerable<ShopifySharp.Product>> GetProductsAsync()
        {
            var productService = CreateService<ShopifySharp.ProductService>();
            IEnumerable<ShopifySharp.Product> products = (await productService.ListAsync()).Items;
            return products;
        }
        public async Task<IEnumerable<ShopifySharp.CustomCollection>> GetCollectionsAsync()
        {
            try
            {
            var customCollectionService = CreateService<ShopifySharp.CustomCollectionService>();
            IEnumerable<ShopifySharp.CustomCollection> collections = (await customCollectionService.ListAsync()).Items;
            return collections;
            }
            catch(Exception ex){
                Console.Write(ex);
                throw;
            }
        }
        public async Task<IEnumerable<ShopifySharp.Product>> GetCollectionsProductsAsync(long collectionId)
        {
            var collectionService = CreateService<ShopifySharp.CollectionService>();
            IEnumerable<ShopifySharp.Product> collectionProducts = (await collectionService.ListProductsAsync(collectionId)).Items;
            return collectionProducts;
        }
        public async Task<IEnumerable<Order>> GetProductsSoldCountAsync(IList<long?> productIds, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
        {
            var ordersInfo = new List<Order>();
            
            var filter = CreateFilter(startDate, endDate);

            var orderService = CreateService<ShopifySharp.OrderService>();
            IEnumerable<ShopifySharp.Order> orders = (await orderService.ListAsync(filter)).Items;

            var ordersCount = CreateCountedOrdersDict(orders, productIds);


            foreach (var product in ordersCount)
            {
                ordersInfo.Add(new Order()
                {
                    ProductId = product.Key,
                    Count = product.Value
                });
            }

            return ordersInfo;
        }

        private T CreateService<T>()
        {
            var url = _config["ShopifyAPI:URL"];
            var accessToken = _config["ShopifyAPI:AccessToken"];

            switch (typeof(T))
            {
                case Type t when t == typeof(ShopifySharp.OrderService):
                    return (T)(object)new ShopifySharp.OrderService(url, accessToken);
                case Type t when t == typeof(ShopifySharp.ProductService):
                    return (T)(object)new ShopifySharp.ProductService(url, accessToken);
                case Type t when t == typeof(ShopifySharp.CustomCollectionService):
                    return (T)(object)new ShopifySharp.CustomCollectionService(url, accessToken);
                case Type t when t == typeof(ShopifySharp.CollectionService):
                    return (T)(object)new ShopifySharp.CollectionService(url, accessToken);
                default:
                    throw new ArgumentException("Unsupported service type");
            }
        }

        private IDictionary<long, int> CreateCountedOrdersDict(IEnumerable<ShopifySharp.Order> orders, IList<long?> productIds)
        {

            IDictionary<long, int> ordersCount = new Dictionary<long, int>();

            foreach (var order in orders)
            {
                foreach (var lineItem in order.LineItems)
                {
                    if (productIds.Contains(lineItem.ProductId))
                    {
                        long productId = (long)lineItem.ProductId!;

                        if (ordersCount.ContainsKey(productId))
                        {
                            ordersCount[productId]++;
                        }
                        else
                        {
                            ordersCount.Add(productId, 1);
                        }
                    }
                }
            }

            return ordersCount;
        }
        private ShopifySharp.Filters.OrderListFilter CreateFilter(DateTimeOffset? startDate, DateTimeOffset? endDate)
        {

            if (startDate == null)
            {
                startDate = DateTimeOffset.Now;
            }
            if (endDate == null)
            {
                endDate = DateTimeOffset.Now.AddDays(-7);
            }

            var filter = new ShopifySharp.Filters.OrderListFilter()
            {
                CreatedAtMax = startDate,
                CreatedAtMin = endDate,
                Status = "open",
                FinancialStatus = "paid",
                FulfillmentStatus = "any"
            };
            return filter;
        }

        public void Dispose()
        {

        }
    }
}