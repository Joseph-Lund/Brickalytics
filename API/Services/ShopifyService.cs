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
        public async Task<List<Collection>> GetCollectionsAsync()
        {
            try
            {
                var customCollectionService = CreateService<ShopifySharp.CustomCollectionService>();
                var smartCollectionService = CreateService<ShopifySharp.SmartCollectionService>();
                IEnumerable<ShopifySharp.CustomCollection> customCollections = (await customCollectionService.ListAsync()).Items;
                IEnumerable<ShopifySharp.SmartCollection> smartCollections = (await smartCollectionService.ListAsync()).Items;
                List<Collection> collections = customCollections
                .Select(collection => new Collection
                {
                    Id = collection.Id,
                    Title = collection.Title
                })
                .Concat(smartCollections
                .Select(collection => new Collection
                {
                    Id = collection.Id,
                    Title = collection.Title
                })).ToList();
                return collections;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw;
            }
        }
        public async Task<List<Order>> GetCreatorsAnalyticsAsync(User user, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
        {
            //TODO: REMOVE THIS
            if(user.CollectionId == null){
            user.CollectionId = 199995129965;
            }

            var products = await GetCollectionsProductsAsync((long)user.CollectionId);

            Dictionary<long, Order> analyticsDict = products
            .ToDictionary(
                product => (long)product.Id!,
                product => new Order { ProductId = (long)product.Id!, Name = product.Title, ProductType = (ProductTypes)GetProductTypeId(product.ProductType) }
            );

            var productDictionary = GetProductsSoldCountAsync(analyticsDict, startDate, endDate);
            var analytics = analyticsDict.Select(analytic => analytic.Value).ToList();
            return analytics;
        }
        private async Task<IDictionary<long, Order>> GetProductsSoldCountAsync(IDictionary<long, Order> analyticsDict, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
        {
            var ordersInfo = new List<Order>();

            IEnumerable<ShopifySharp.Order> orders = await GetCollectionOrders();
            analyticsDict = GetOrderCountsDict(orders, analyticsDict);

            return analyticsDict;
        }
        private async Task<IEnumerable<ShopifySharp.Product>> GetCollectionsProductsAsync(long collectionId)
        {
            var collectionService = CreateService<ShopifySharp.CollectionService>();
            IEnumerable<ShopifySharp.Product> collectionProducts = (await collectionService.ListProductsAsync(collectionId)).Items;
            return collectionProducts;
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
                case Type t when t == typeof(ShopifySharp.SmartCollectionService):
                    return (T)(object)new ShopifySharp.SmartCollectionService(url, accessToken);
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
                        decimal price = (decimal)lineItem.Price!;
                        if (analyticsDict.ContainsKey(productId))
                        {
                            analyticsDict[productId].Count++;
                        }
                        else
                        {
                            analyticsDict.Add(productId, new Order() { ProductId = productId, Count = 1, Price = price });
                        }
                    }
                }
            }

            return analyticsDict;
        }
        private ShopifySharp.Filters.OrderListFilter CreateOrderListFilter(DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
        {

            if (startDate == null)
            {
                startDate = new DateTime(2023, 5, 7);
            }
            if (endDate == null)
            {
                endDate = new DateTime(2023, 5, 9);
            }

            var filter = new ShopifySharp.Filters.OrderListFilter()
            {
                CreatedAtMax = startDate,
                CreatedAtMin = endDate,
                Status = "any",
                FinancialStatus = "paid",
                FulfillmentStatus = "any",
                Limit = 250
            };
            return filter;
        }
        private ShopifySharp.Filters.OrderCountFilter CreateOrderCountFilter(DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
        {

            if (startDate == null)
            {
                startDate = new DateTime(2023, 5, 7);
            }
            if (endDate == null)
            {
                endDate = new DateTime(2023, 5, 9);
            }

            var filter = new ShopifySharp.Filters.OrderCountFilter()
            {
                CreatedAtMax = startDate,
                CreatedAtMin = endDate,
                Status = "any",
                FinancialStatus = "paid",
                FulfillmentStatus = "any"
            };
            return filter;
        }

        private int GetProductTypeId(string name)
        {
            if (Enum.GetNames(typeof(ProductTypes)).Contains(name))
            {
                return (int)Enum.Parse(typeof(ProductTypes), name);
            }
            else
            {
                return 0;
            }
        }
        private async Task<List<ShopifySharp.Order>> GetCollectionOrders()
        {

            var orderService = CreateService<ShopifySharp.OrderService>();
            var allOrders = new List<ShopifySharp.Order>();

            var page = await orderService.ListAsync(CreateOrderListFilter());

            while (true)
            {
                allOrders.AddRange(page.Items);

                if (!page.HasNextPage)
                {
                    break;
                }

                page = await orderService.ListAsync(CreateOrderListFilter());
            }
            return allOrders;


        }
        public void Dispose()
        {

        }
    }
}