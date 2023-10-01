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
        public async Task<List<Order>> GetCreatorsAnalyticsAsync(User user, List<UserRate> rates, DateTime startDate, DateTime endDate)
        {
            var products = await GetCollectionsProductsAsync((long)user.CollectionId, rates, startDate, endDate);
            return products;
        }
        private async Task<List<Order>> GetProductsSoldCountAsync(List<Order> analyticsDict, DateTime startDate, DateTime endDate)
        {
            var ordersInfo = new List<Order>();

            IEnumerable<ShopifySharp.Order> orders = await GetAllOrders(startDate, endDate);
            analyticsDict = GetOrderCountsDict(orders, analyticsDict);

            return analyticsDict;
        }

        private async Task<List<ShopifySharp.Order>> GetAllOrders(DateTime startDate, DateTime endDate)
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
        private async Task<List<Order>> GetCollectionsProductsAsync(long collectionId, List<UserRate> rates, DateTime startDate, DateTime endDate)
        {
            var collectionService = CreateService<ShopifySharp.CollectionService>();
            var productVariantService = CreateService<ShopifySharp.ProductVariantService>();
            var collections = await collectionService.ListProductsAsync(collectionId);
            List<Order> collectionProducts = new List<Order>();

            foreach (var product in collections.Items)
            {
                // has a rate
                bool productTypeIdExists = rates.Any(rate => (rate.ProductTypeId == GetProductTypeId(product.ProductType)));

                if (productTypeIdExists)
                {
                    collectionProducts.Add(new Order { ProductId = (long)product.Id!, Name = product.Title, ProductType = (ProductTypes)GetProductTypeId(product.ProductType), ProductTypeId = GetProductTypeId(product.ProductType), Rate = GetRate(rates, GetProductTypeId(product.ProductType)) });

                }
            }
            collectionProducts = await GetProductsSoldCountAsync(collectionProducts, startDate, endDate);
            return collectionProducts;
        }
        private List<Order> GetOrderCountsDict(IEnumerable<ShopifySharp.Order> orders, List<Order> analytics)
        {
            // orders = orders.Where(x => x.Refunds.Count() > 0);
            foreach (var order in orders)
            {
                foreach (var lineItem in order.LineItems)
                {
                    if (lineItem.ProductId != null)
                    {
                        foreach (var analytic in analytics)
                        {
                            if (analytic.ProductId == (long)lineItem.ProductId!)
                            {
                                var priceTotal = 0.0;
                                var variant = (long)Convert.ToDouble(lineItem.VariantId);
                                // Shammy
                                if (variant == 44364937789755)
                                {
                                    priceTotal = 25.00;
                                }
                                else if (variant == 44364926386491)
                                {

                                    priceTotal = 25.00;
                                }
                                else if (variant == 44364926386491)
                                {

                                    priceTotal = 25.00;
                                }
                                else if (variant == 44364945162555)
                                {

                                    priceTotal = 57.50;
                                }
                                else if (variant == 44364945031483)
                                {

                                    priceTotal = 55.00;
                                }
                                // Dicehammer
                                else if (variant == 45192255340859)
                                {

                                    priceTotal = 55.00;
                                }
                                else if (variant == 45192422588731)
                                {

                                    priceTotal = 25.00;
                                }
                                else if (variant == 45192495169851)
                                {

                                    priceTotal = 25.00;
                                }
                                else if (variant == 45192422687035)
                                {

                                    priceTotal = 25.00;
                                }
                                else if (variant == 5308663628091)
                                {

                                    priceTotal = 25.00;
                                }
                                else if (variant == 45192422555963)
                                {

                                    priceTotal = 25.00;
                                }
                                else if (variant == 44364926124347)
                                {

                                    priceTotal = 25.00;
                                }
                                decimal price = (decimal)lineItem.Price!;
                                analytic.Price = priceTotal == 0.0 ? (decimal)lineItem.Price : (decimal)priceTotal;
                                analytic.Rate = analytic.Rate;
                                analytic.Count++;
                            }
                        }
                    }
                }
            }

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
                case Type t when t == typeof(ShopifySharp.SmartCollectionService):
                    return (T)(object)new ShopifySharp.SmartCollectionService(url, accessToken);
                case Type t when t == typeof(ShopifySharp.CollectionService):
                    return (T)(object)new ShopifySharp.CollectionService(url, accessToken);
                case Type t when t == typeof(ShopifySharp.ProductVariantService):
                    return (T)(object)new ShopifySharp.ProductVariantService(url, accessToken);
                default:
                    throw new ArgumentException("Unsupported service type");
            }
        }
        private ShopifySharp.Filters.OrderListFilter CreateOrderListFilter(DateTimeOffset startDate, DateTimeOffset endDate)
        {
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
        private decimal GetRate(List<UserRate> rates, int productTypeId)
        {
            foreach (var rate in rates)
            {
                if (rate.ProductTypeId == productTypeId)
                {
                    return Convert.ToDecimal(rate.Rate);
                }
            }
            return (decimal)0.0;
        }

        public void Dispose()
        {

        }
    }
}