using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaoZhugong.Models.DbEntities;

namespace TaoZhugong.Models
{
    public class ProductRepository : IProductRepository
    {
        ITaoZhugongDatabaseConnection dbConnection;
        public ProductRepository()
        {
            dbConnection = new TaoZhugongDatabaseConnection();
        }

        public ProductRepository(ITaoZhugongDatabaseConnection _dbConnection)
        {
            dbConnection = _dbConnection;
        }

        public IQueryable<Product> GetProductList()
        {
            return dbConnection.QueryableProduct;
        }
    }
}