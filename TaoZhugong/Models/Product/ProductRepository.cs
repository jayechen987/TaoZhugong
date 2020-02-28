using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public string EditProduct(Product product)
        {

            try
            {
                dbConnection.Modified(product, EntityState.Added);
                dbConnection.SaveChanges();

                return "Success";
            }
            catch (Exception ee)
            {
                return ee.Message;
            }
        }
    }
}