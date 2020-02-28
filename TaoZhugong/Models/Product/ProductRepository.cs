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
        private IAssetRepository assetRepository;
        public ProductRepository()
        {
            dbConnection = new TaoZhugongDatabaseConnection();
            assetRepository = new AssetRepository();
        }

        public ProductRepository(ITaoZhugongDatabaseConnection _dbConnection, IAssetRepository _asset)
        {
            dbConnection = _dbConnection;
            assetRepository = _asset;

        }

        public IQueryable<Product> GetProductList()
        {
            return dbConnection.QueryableProduct;
        }

        public string EditProduct(Product product)
        {
            var isNewProduct = product.ProductSeq == 0;
            if (!isNewProduct)
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
                if (isNewProduct)
                {
                    assetRepository.AddNewAsset(product);
                }

                return "Success";
            }
            catch (Exception ee)
            {
                return ee.Message;
            }
        }
    }
}