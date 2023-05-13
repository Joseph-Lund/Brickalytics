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
        public async Task<ShopifySharp.Product> GetProductByIdAsync(long productId)
        {
            var productService = CreateService<ShopifySharp.ProductService>();
            ShopifySharp.Product product = await productService.GetAsync(productId);
            return product;
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
        public async Task<IDictionary<long, Order>> GetProductsSoldCountAsync(IDictionary<long, Order> analyticsDict, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
        {
            var ordersInfo = new List<Order>();
            
            var filter = CreateFilter(startDate, endDate);

            var orderService = CreateService<ShopifySharp.OrderService>();
            IEnumerable<ShopifySharp.Order> orders = (await orderService.ListAsync(filter)).Items;

            analyticsDict = GetOrderCountsDict(orders, analyticsDict);
            return analyticsDict;
        }
        public async Task<List<Order>> GetCreatorsAnalytics(long collectionId, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
        {
            var products = await GetCollectionsProductsAsync(collectionId);

            Dictionary<long, Order> analyticsDict = products
            .ToDictionary(
                product => (long)product.Id!,
                product => new Order { ProductId = (long)product.Id!, Name = product.Title, ProductTypeId = GetProductType(product.ProductType)}
            );

            var productDictionary = GetProductsSoldCountAsync(analyticsDict, startDate, endDate);
            var analytics = analyticsDict.Select(analytic => analytic.Value).ToList();
            return analytics;
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

        private IDictionary<long, Order> GetOrderCountsDict(IEnumerable<ShopifySharp.Order> orders, IDictionary<long, Order> analyticsDict)
        {
            var productIds = analyticsDict.Keys.ToList();

            foreach (var order in orders)
            {
                foreach (var lineItem in order.LineItems)
                {
                    if (productIds.Contains((long)lineItem.ProductId!))
                    {
                        long productId = (long)lineItem.ProductId!;

                        if (analyticsDict.ContainsKey(productId))
                        {
                            analyticsDict[productId].Count++;
                        }
                        else
                        {
                            analyticsDict.Add(productId, new Order(){ProductId = productId, Count = 1});
                        }
                    }
                }
            }

            return analyticsDict;
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

        private int GetProductType(string name)
        {
            if(Enum.GetNames(typeof(ProductTypes)).Contains(name))
            {
                return (int)Enum.Parse(typeof(ProductTypes), name);
            }
            else
            {
                return 0;
            }
        }
        public void Dispose()
        {

        }
    }
}