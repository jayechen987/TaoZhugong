using System.Collections.Generic;
using System.Linq;
using TaoZhugong.Models.DbEntities;

namespace TaoZhugong.Models
{
    public interface IAssetRepository
    {
        /// <summary>
        /// 建立完產品後會新增該產品的資產資料供日後計算
        /// </summary>
        /// <param name="product"></param>
        void AddNewAsset(Product product);

        IEnumerable<Asset> GetAssetListByType(string type);

        /// <summary>
        /// 根據資產取均價(不含手續費)
        /// 計算到小數點2位
        /// </summary>
        /// <param name="asset">單筆資產</param>
        /// <returns></returns>
        double GetAveragePrice(Asset asset);

        /// <summary>
        /// 損益平衡點
        /// 計算到小數點後2位
        /// </summary>
        /// <param name="productTrans">持有的交易紀錄(尚未售出)</param>
        /// <returns></returns>
        double GetBreakevenPoint(IQueryable<TransactionRecord> productTrans);

    }
}

