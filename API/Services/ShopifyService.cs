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
            var customCollectionService = CreateService<ShopifySharp.CustomCollectionService>();
            var smartCollectionService = CreateService<ShopifySharp.SmartCollectionService>();
            IEnumerable<ShopifySharp.CustomCollection> customCollections = (await customCollectionService.ListAsync()).Items;
            IEnumerable<ShopifySharp.SmartCollection> smartCollections = (await smartCollectionService.ListAsync()).Items;
            List<Collection> collections = customCollections
            .Select(collection => new Collection
            {
                Id = collection.Id,
                Name = collection.Title
            })
            .Concat(smartCollections
            .Select(collection => new Collection
            {
                Id = collection.Id,
                Name = collection.Title
            })).ToList();
            return collections;
        }
        public async Task<List<Order>> GetCreatorsAnalyticsAsync(User user, DateTime? startDate, DateTime? endDate)
        {
            //TODO: If user is not normal user, then get all the users collection ids
            if (user.CollectionId == null)
            {
                user.CollectionId = 199995129965;
            }

            var products = await GetCollectionsProductsAsync((long)user.CollectionId, startDate, endDate);

            var analytics = products.Select(analytic => analytic.Value).ToList();
            return analytics;
        }
        private async Task<IDictionary<long, Order>> GetProductsSoldCountAsync(IDictionary<long, Order> analyticsDict, DateTime? startDate, DateTime? endDate)
        {
            var ordersInfo = new List<Order>();

            IEnumerable<ShopifySharp.Order> orders = await GetCollectionOrders(startDate, endDate);
            analyticsDict = GetOrderCountsDict(orders, analyticsDict);

            return analyticsDict;
        }

        private async Task<List<ShopifySharp.Order>> GetCollectionOrders(DateTime? startDate, DateTime? endDate)
        {

            var orderService = CreateService<ShopifySharp.OrderService>();
            var allOrders = new List<ShopifySharp.Order>();
            ShopifySharp.Lists.ListResult<ShopifySharp.Order> page;

            page = await orderService.ListAsync(CreateOrderListFilter(startDate, endDate));

            while (true)
            {
                allOrders.AddRange(page.Items);

                if (!page.HasNextPage)
                {
                    break;
                }

                page = await orderService.ListAsync(page.GetNextPageFilter());
            }
            return allOrders;
        }
        private async Task<IDictionary<long, Order>> GetCollectionsProductsAsync(long collectionId, DateTime? startDate, DateTime? endDate)
        {
            var collectionService = CreateService<ShopifySharp.CollectionService>();
            var collections = await collectionService.ListProductsAsync(collectionId);
            IDictionary<long, Order> collectionProducts = (collections).Items
            .ToDictionary(
                product => (long)product.Id!,
                product => new Order {  
                    ProductId = (long)product.Id!, 
                    Name = product.Title, 
                    ProductType = (ProductTypes)GetProductTypeId(product.ProductType)
                }
            );
            collectionProducts = await GetProductsSoldCountAsync(collectionProducts, startDate, endDate);
            return collectionProducts;
        }
        private IDictionary<long, Order> GetOrderCountsDict(IEnumerable<ShopifySharp.Order> orders, IDictionary<long, Order> analyticsDict)
        {
            var productIds = analyticsDict.Keys.ToList();
            foreach (var order in orders)
            {
                foreach (var lineItem in order.LineItems)
                {
                    if (lineItem.ProductId != null)
                    {
                        if (productIds.Contains((long)lineItem.ProductId!))
                        {
                            long productId = (long)lineItem.ProductId!;
                            decimal price = (decimal)lineItem.Price!;
                            if (analyticsDict.ContainsKey(productId))
                            {
                                analyticsDict[productId].Price = price;
                                analyticsDict[productId].Count++;
                            }
                            else
                            {
                                analyticsDict.Add(productId, new Order() { ProductId = productId, Count = 1, Price = price });
                            }
                        }
                    }
                }
            }

            return analyticsDict;

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
        private ShopifySharp.Filters.OrderListFilter CreateOrderListFilter(DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            var defaultDate = new DateTime(2023, 5, 6, 0, 0, 0);

            if (startDate == null)
            {
                startDate = defaultDate;
            }
            if (endDate == null)
            {
                endDate = defaultDate.AddDays(1).AddSeconds(-1);
            }

            var filter = new ShopifySharp.Filters.OrderListFilter()
            {
                Limit = 250,
                CreatedAtMin = startDate,
                CreatedAtMax = endDate,
                Status = "any",
                FinancialStatus = "paid",
                FulfillmentStatus = "shipped"
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

        public void Dispose()
        {

        }
    }
}