using Brickalytics.Models;

namespace Brickalytics.Services
{
    public interface IShopifyService : IDisposable
    {
        Task<IEnumerable<ShopifySharp.Order>> GetOrdersAsync();
        Task<IEnumerable<ShopifySharp.Product>> GetProductsAsync();
        Task<ShopifySharp.Product> GetProductByIdAsync(long productId);
        Task<IEnumerable<ShopifySharp.CustomCollection>> GetCollectionsAsync();
        Task<IEnumerable<ShopifySharp.Product>> GetCollectionsProductsAsync(long collectionId);
        Task<IDictionary<long, Order>> GetProductsSoldCountAsync(IDictionary<long, Order> analyticsDict, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null);
        Task<List<Order>> GetCreatorsAnalytics(long collectionId, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null);

    }    
}    