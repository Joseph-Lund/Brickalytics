using Brickalytics.Models;

namespace Brickalytics.Services
{    
    public interface IProductSubTypeService : IDisposable    
    {    
        Task<List<ProductSubType>> GetProductSubTypesAsync();
    }    
}    