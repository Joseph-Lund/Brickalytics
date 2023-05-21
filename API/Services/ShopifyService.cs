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
        public async Task<List<Order>> GetCreatorsAnalyticsAsync(User user, List<UserRate> rates, DateTime? startDate, DateTime? endDate)
        {
            //TODO: If user is not normal user, then get all the users collection ids
            // if (user.CollectionId == 0)
            // {
            //     user.CollectionId = 199995129965;
            // }

            var products = await GetCollectionsProductsAsync((long)user.CollectionId, rates, startDate, endDate);
            return products;
        }
        private async Task<List<Order>> GetProductsSoldCountAsync(List<Order> analyticsDict, DateTime? startDate, DateTime? endDate)
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
        private async Task<List<Order>> GetCollectionsProductsAsync(long collectionId, List<UserRate> rates, DateTime? startDate, DateTime? endDate)
        {
            var collectionService = CreateService<ShopifySharp.CollectionService>();
            var productVariantService = CreateService<ShopifySharp.ProductVariantService>();
            var collections = await collectionService.ListProductsAsync(collectionId);
            List<Order> collectionProducts = new List<Order>();

            var productTypeIdsWithNoSubs = rates
                .Where(rate => rate.ProductSubTypeId == 0)
                .Select(rate => rate.ProductTypeId)
                .Distinct()
                .ToList();

            foreach (var product in collections.Items)
            {
                // has a rate
                bool productTypeIdExists = rates.Any(rate => (rate.ProductTypeId == GetProductTypeId(product.ProductType)));
                bool productHasNoSubs = productTypeIdsWithNoSubs.Contains(GetProductTypeId(product.ProductType));

                if (productTypeIdExists)
                {
                    if (!productHasNoSubs)
                    {
                        var variants = (await productVariantService.ListAsync((long)product.Id)).Items;
                        if (variants.Count() > 0)
                        {
                            foreach (var variaint in variants)
                            {
                                collectionProducts.Add(new Order { ProductId = (long)product.Id!, Name = product.Title, ProductType = (ProductTypes)GetProductTypeId(product.ProductType), ProductTypeId = GetProductSubTypeId(variaint.Option1), Rate = GetRate(rates, GetProductTypeId(product.ProductType), GetProductSubTypeId(variaint.Option1)) });

                            }

                        }
                    }
                    else
                    {
                        collectionProducts.Add(new Order { ProductId = (long)product.Id!, Name = product.Title, ProductType = (ProductTypes)GetProductTypeId(product.ProductType), ProductTypeId = 0, Rate = GetRate(rates, GetProductTypeId(product.ProductType), 0) });
                    }
                }
            }
            collectionProducts = await GetProductsSoldCountAsync(collectionProducts, startDate, endDate);
            return collectionProducts;
        }
        private List<Order> GetOrderCountsDict(IEnumerable<ShopifySharp.Order> orders, List<Order> analytics)
        {
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
                                long productId = (long)lineItem.ProductId;
                                decimal price = (decimal)lineItem.Price!;
                                analytic.Price = price;
                                analytic.Rate = analytic.Price;
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

        private int GetProductSubTypeId(string name)
        {
            switch (name)
            {
                case "8x10 in.":
                    return 19;
                case "11x14 in.":
                    return 20;
                case "18x24 in.":
                    return 21;
                case "24x36 in.":
                    return 22;
                default:
                    return 0;
            }
        }

        private decimal GetRate(List<UserRate> rates, int productTypeId, int productSubTypeId)
        {
            foreach(var rate in rates){
                if(rate.ProductTypeId == productTypeId && rate.ProductTypeId == productSubTypeId){
                    return (decimal)rate.Rate;
                }
            }
            return (decimal)0.0;
        }

        public void Dispose()
        {

        }
    }
}