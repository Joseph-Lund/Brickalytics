using Brickalytics.Models;

namespace Brickalytics.Services
{
    public class ProductSubTypeService : IProductSubTypeService
    {
        private readonly ILogger<ProductSubTypeService> _logger;
        private readonly IDapperService _dapper;

        public ProductSubTypeService(ILogger<ProductSubTypeService> logger, IDapperService dapper)
        {
            _logger = logger;
            _dapper = dapper;
        }
        public async Task<List<ProductSubType>> GetProductSubTypesAsync()
        {
            var result = await Task.FromResult(_dapper.GetAll<ProductSubType>("GetProductSubTypes"));
            return result;
        }
        
        public void Dispose()
        {

        }

    }
}