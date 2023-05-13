using Brickalytics.Models;

namespace Brickalytics.Services
{
    public interface IShopifyService : IDisposable
    {
        Task<IEnumerable<ShopifySharp.Order>> GetOrdersAsync();
        Task<IEnumerable<ShopifySharp.Product>> GetProductsAsync();
        Task<IEnumerable<ShopifySharp.CustomCollection>> GetCollectionsAsync();
        Task<IEnumerable<ShopifySharp.Product>> GetCollectionsProductsAsync(long collectionId);
        Task<IEnumerable<Order>> GetProductsSoldCountAsync(IList<long?> productIds, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null);

    }    
}    