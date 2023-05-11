using Brickalytics.Models;

namespace Brickalytics.Services
{    
    public interface IProductTypeService : IDisposable    
    {    
        Task<List<ProductType>> GetProductTypesAsync();
    }    
}    