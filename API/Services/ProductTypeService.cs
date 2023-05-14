using Brickalytics.Helpers;
using Brickalytics.Models;

namespace Brickalytics.Services
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly ILogger<ProductTypeService> _logger;
        private readonly IDapperHelper _dapper;

        public ProductTypeService(ILogger<ProductTypeService> logger, IDapperHelper dapper)
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