using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaoZhugong.Models.DbEntities;

namespace TaoZhugongTests.FakeData
{
    public static class FakeTransactionRecord
    {
        /// <summary>
        /// 所有交易紀錄
        /// </summary>
        public static List<TransactionRecord> transactionRecorList => new List<TransactionRecord>()
        {
            StockProduct1_TX_1,
            StockProduct1_TX_2,
            StockProduct2_TX_1,
            StockProduct2_TX_2,
        };
        /// <summary>
        /// 計算各項數值用的清單
        /// </summary>
        public static List<TransactionRecord> StockProduct1_TXList => new List<TransactionRecord>()
        {
            StockProduct1_TX_1,
            StockProduct1_TX_2,
        };
        /// <summary>
        /// 有售出紀錄的清單
        /// </summary>
        public static List<TransactionRecord> StockProduct2_TXList => new List<TransactionRecord>()
        {
            StockProduct2_TX_1,
            StockProduct2_TX_2,
        };

        /// <summary>
        /// 計算各項數值用的清單
        /// </summary>
        public static List<TransactionRecord> StockProduct1_WithDividends_TXList => new List<TransactionRecord>()
        {
            StockProduct1_TX_1,
            StockProduct1_TX_2,
        };

        public static TransactionRecord StockProduct1_TX_1 => new TransactionRecord()
        {
            Seq = 1,
            ProductSeq = FakeProduct.StockProduct1.ProductSeq,
            Num = 1000,
            InStock = 1000,
            UnitPrice = 10,
            TotalPrice = 10000,
            TransactionTime = new DateTime(2019, 02, 01),
        };

        public static TransactionRecord StockProduct1_TX_2 => new TransactionRecord()
        {
            Seq = 2,
            ProductSeq = FakeProduct.StockProduct1.ProductSeq,
            Num = 1000,
            InStock = 1000,
            UnitPrice = 30,
            TotalPrice = 30000,
            AdministractionFee = 10000,
            TransactionTime = new DateTime(2019, 02, 02),
        };
        

        public static TransactionRecord StockProduct2_TX_1 => new TransactionRecord()
        {
            Seq = 3,
            ProductSeq = FakeProduct.StockProduct2.ProductSeq,
            Num = 1000,
            InStock = 1000,
            UnitPrice = 20,
            TotalPrice = 20000,
            TransactionTime = new DateTime(2019, 02, 02),

        };
        /// <summary>
        /// 售出的交易紀錄
        /// </summary>
        public static TransactionRecord StockProduct2_TX_2 => new TransactionRecord()
        {
            Seq = 4,
            ProductSeq = FakeProduct.StockProduct2.ProductSeq,
            Num = 1000,
            InStock = 0,
            UnitPrice = 10,
            TotalPrice = 10000,
            SalePrice = 20,
            SaleTax = 100,
            TransactionTime = new DateTime(2019, 02, 02),
        };
        /// <summary>
        /// 零股紀錄
        /// </summary>
        public static TransactionRecord StockProduct1_TX_3 => new TransactionRecord()
        {
            Seq = 5,
            ProductSeq = FakeProduct.StockProduct1.ProductSeq,
            Num = 200,
            InStock = 200,
            UnitPrice = 0,
            TotalPrice = 0,
            AdministractionFee = 0,
            TransactionTime = new DateTime(2019, 02, 05),
        };
        

    }
}
