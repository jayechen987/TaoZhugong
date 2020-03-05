using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TaoZhugong.Models.CustomerException;
using TaoZhugong.Models.DbEntities;

namespace TaoZhugong.Models
{
    public class AssetRepository :  IAssetRepository
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
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                };

                dbConnection.Modified(newAsset, EntityState.Added);
                dbConnection.SaveChanges();
            }

        }

        public IEnumerable<Asset> GetAssetListByType(string type)
        {
            var productList = dbConnection.QueryableProducts.Where(p => p.Type == type).ToList();
            var productIdList = productList.Select(p => p.ProductSeq).ToList();
            var holdTrans = dbConnection.QueryableTransactionRecords.Where(p => p.SalePrice == null);
            var returnList = dbConnection.QueryableAssets.Where(p => productIdList.Contains(p.ProductSeq) && p.Num > 0).ToList().Select(p =>
            {
                p.ProductName = !productList.Any(q => q.ProductSeq == p.ProductSeq) ? "查無產品" :
                    productList.FirstOrDefault(q => q.ProductSeq == p.ProductSeq).ProductName;
                p.AveragePrice = GetAveragePrice(p);
                p.BreakevenPoint = GetBreakevenPoint(holdTrans.Where(q => q.ProductSeq == p.ProductSeq));
                return p;
            });

            return returnList;
        }

        #region Tool

        /// <summary>
        /// 根據資產取均價(不含手續費)
        /// 計算到小數點2位
        /// </summary>
        /// <param name="asset">單筆資產</param>
        /// <returns></returns>
        public double GetAveragePrice(Asset asset)
        {
            return asset.Num == 0 ? 0 : Math.Round(asset.TotalPrice / asset.Num, 2);
        }

        /// <summary>
        /// 損益平衡點
        /// 計算到小數點後2位
        /// </summary>
        /// <param name="productTrans">持有的交易紀錄(尚未售出)</param>
        /// <returns></returns>
        public double GetBreakevenPoint(IQueryable<TransactionRecord> productTrans)
        {
            return productTrans.Sum(p => p.InStock) == 0 ? 0 :
                Math.Round(
                    (productTrans.Sum(p => p.TotalPrice) + productTrans.Sum(p => p.AdministractionFee))
                    / productTrans.Sum(p => p.InStock)
                    , 2);
        }

        private bool CheckAssetIsNew(Product ProductData)
        {
            return !dbConnection.QueryableAssets.Any(p => p.ProductSeq == ProductData.ProductSeq);
        }

        private void CheckProductIsNull(Product ProductData)
        {
            if (!dbConnection.QueryableProducts.Any(p => p.ProductSeq == ProductData.ProductSeq))
            {
                throw new DataNotFoundException();
            }
        }
        #endregion Tool

    }
}