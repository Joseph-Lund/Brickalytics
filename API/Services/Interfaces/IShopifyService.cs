using Brickalytics.Models;

namespace Brickalytics.Services
{
    public interface IShopifyService : IDisposable
    {
        Task<IEnumerable<ShopifySharp.Order>> GetOrdersAsync();
        Task<IEnumerable<ShopifySharp.Product>> GetProductsAsync();
        Task<ShopifySharp.Product> GetProductByIdAsync(long productId);
        Task<List<Collection>> GetCollectionsAsync();
        Task<List<Order>> GetCreatorsAnalyticsAsync(User userId, List<UserRate> rates, DateTime startDate, DateTime endDate);

    }    
}    