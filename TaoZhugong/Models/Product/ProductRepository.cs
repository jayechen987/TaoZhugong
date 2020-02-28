using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TaoZhugong.Models.CustomerException;
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
            if (product.ProductSeq != 0)
            {
                var oldData = dbConnection.QueryableProduct.FirstOrDefault(p => p.ProductSeq == product.ProductSeq);
                if (oldData==null)
                {
                    throw new DataNotFoundException();
                }

                oldData.ProductName = product.ProductName;
                oldData.Remark = product.Remark;
                oldData.Owner = product.Owner;
                
                dbConnection.Modified(oldData, EntityState.Modified);
            }
            else
            {
                dbConnection.Modified(product, EntityState.Added);
            }
            try
            {
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