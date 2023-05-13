using Brickalytics.Models;

namespace Brickalytics.Services
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly ILogger<ProductTypeService> _logger;
        private readonly IDapperService _dapper;

        public ProductTypeService(ILogger<ProductTypeService> logger, IDapperService dapper)
        {
            _logger = logger;
            _dapper = dapper;
        }
        public async Task<List<ProductType>> GetProductTypesAsync()
        {
            var result = await Task.FromResult(_dapper.GetAll<ProductType>("GetProductTypes"));
            return result;
        }
        
        public void Dispose()
        {

        }

    }
}