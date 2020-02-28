using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TaoZhugong.Models.CustomerException;
using TaoZhugong.Models.DbEntities;

namespace TaoZhugong.Models
{
    public class AssetRepository : IAssetRepository
    {
        private ITaoZhugongDatabaseConnection dbConnection;
        public AssetRepository()
        {
            dbConnection = new TaoZhugongDatabaseConnection();
        }

        public AssetRepository(ITaoZhugongDatabaseConnection _dbConnection)
        {
            dbConnection = _dbConnection;
        }

        /// <summary>
        /// 建立完產品後會新增該產品的資產資料供日後計算
        /// </summary>
        /// <param name="product"></param>
        public void AddNewAsset(Product ProductData)
        {
            CheckProductIsNull(ProductData);

            if (CheckAssetIsNew(ProductData))
            {
                var newAsset = new Asset()
                {
                    ProductSeq = ProductData.ProductSeq,
                    Num = 0,
                    TotalPrice = 0,
                    CreatTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                };

                dbConnection.Modified(newAsset, EntityState.Added);
                dbConnection.SaveChanges();
            }

        }

        private bool CheckAssetIsNew(Product ProductData)
        {
            return !dbConnection.QueryableAsset.Any(p => p.ProductSeq == ProductData.ProductSeq);
        }

        private void CheckProductIsNull(Product ProductData)
        {
            if (!dbConnection.QueryableProduct.Any(p => p.ProductSeq == ProductData.ProductSeq))
            {
                throw new DataNotFoundException();
            }
        }
    }
}