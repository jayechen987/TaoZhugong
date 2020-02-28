using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaoZhugong.Models.DbEntities;

namespace TaoZhugongTests.FakeData
{
    public static class FakeAsset
    {
        public static List<Asset> AssetList => new List<Asset>()
        {
            StockProduct1,
            StockProduct2,
        };

        public static Asset StockProduct1 => new Asset()
        {
            Seq = 1,
            ProductSeq = FakeProduct.StockProduct1.ProductSeq,
            Num = FakeTransactionRecord.StockProduct1_TXList.Sum(p => p.Num),
            TotalPrice = FakeTransactionRecord.StockProduct1_TXList.Sum(p => p.TotalPrice),
        };

        /// <summary>
        /// 有售出紀錄的資產
        /// </summary>
        public static Asset StockProduct2 => new Asset()
        {
            Seq = 2,
            ProductSeq = FakeProduct.StockProduct2.ProductSeq,
            Num = FakeTransactionRecord.StockProduct2_TXList.Where(p => p.SalePrice == null).Sum(p => p.Num),
            TotalPrice = FakeTransactionRecord.StockProduct2_TXList.Where(p => p.SalePrice == null).Sum(p => p.TotalPrice),

        };
    }
}
