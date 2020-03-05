using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaoZhugong.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using TaoZhugong.Models.DbEntities;
using TaoZhugong.Models.ViewModel;
using TaoZhugongTests.FakeData;

namespace TaoZhugong.Models.Transaction.Tests
{
    [TestClass()]
    public class TransactionRepositoryTests
    {
        ITaoZhugongDatabaseConnection dbConnection;
        ITransactionRepository transactionRepository;

        private Product product;
        private Asset dbAsset;

        [TestInitialize]
        public void TestInitialize()
        {
            dbConnection = Substitute.For<ITaoZhugongDatabaseConnection>();
            transactionRepository = new TransactionRepository(dbConnection);

            dbConnection.QueryableAssets.ReturnsForAnyArgs(FakeAsset.AssetList.AsQueryable());
            dbConnection.QueryableTransactionRecords.ReturnsForAnyArgs(FakeTransactionRecord.StockProduct1_TXList.AsQueryable());
            product = FakeProduct.StockProduct1;
            dbAsset = FakeAsset.StockProduct1;
        }

        #region 買入

        /// <summary>
        /// 買入單張
        /// </summary>
        [TestMethod()]
        public void AddTransactionLog_BuyStock()
        {
            var txViewModel = SetTransactionViewModel(
                false, product, 1000, 10, 50, 0, false);

            var except = "Success";
            var actual = transactionRepository.AddTransactionLog(txViewModel);
            Assert.AreEqual(except, actual);

            CheckReceived(product, txViewModel, dbAsset);
        }

        /// <summary>
        /// 買入零股_新增
        /// </summary>
        [TestMethod()]
        public void AddTransactionLog_BuyOddLot()
        {
            var txViewModel = SetTransactionViewModel(
                false, product, 300, 10, 50, 0, false);

            var except = "Success";
            var actual = transactionRepository.AddTransactionLog(txViewModel);
            Assert.AreEqual(except, actual);

            CheckReceived(product, txViewModel, dbAsset);
        }

        /// <summary>
        /// 買入零股_更新
        /// </summary>
        [TestMethod()]
        public void AddTransactionLog_BuyOddLot_Update()
        {
            //已有該產品的零股紀錄
            dbConnection.QueryableTransactionRecords.ReturnsForAnyArgs(
                new List<TransactionRecord>() { FakeTransactionRecord.StockProduct1_TX_3 }.AsQueryable());

            var txViewModel = SetTransactionViewModel(
                false, product, 300, 10, 50, 0, false);

            var except = "Success";
            var actual = transactionRepository.AddTransactionLog(txViewModel);
            Assert.AreEqual(except, actual);

            CheckReceived(product, txViewModel, dbAsset, FakeTransactionRecord.StockProduct1_TX_3);
        }

        /// <summary>
        /// 買入零股_更新&新增
        /// </summary>
        [TestMethod()]
        public void AddTransactionLog_BuyOddLot_UpdateAdd()
        {
            //已有該產品的零股紀錄
            dbConnection.QueryableTransactionRecords.ReturnsForAnyArgs(
                new List<TransactionRecord>() { FakeTransactionRecord.StockProduct1_TX_3 }.AsQueryable());

            var txViewModel = SetTransactionViewModel(
                false, product, 900, 10, 50, 0, false);

            var except = "Success";
            var actual = transactionRepository.AddTransactionLog(txViewModel);
            Assert.AreEqual(except, actual);

            CheckReceived(product, txViewModel, dbAsset, FakeTransactionRecord.StockProduct1_TX_3);
        }

        #endregion 買入

        /// <summary>
        /// 配股交易
        /// </summary>
        [TestMethod()]
        public void AddTransactionLog_Dividends()
        {
            var txViewModel = SetTransactionViewModel(false, product, 300, 0, 0, 2000, true);

            var except = "Success";
            var actual = transactionRepository.AddTransactionLog(txViewModel);
            Assert.AreEqual(except, actual);

            CheckReceived(product, txViewModel, dbAsset);
        }

        #region 賣出

        /// <summary>
        /// 賣出整張
        /// </summary>
        [TestMethod()]
        public void AddTransactionLog_SoldStock()
        {
            var txViewModel = SetTransactionViewModel(true, product, 1000, 20, 23, 0, false);
            var tranList = dbConnection.QueryableTransactionRecords.OrderBy(p => p.TransactionTime).Where(p =>
                p.ProductSeq == txViewModel.ProductSeq && p.InStock > 0);
            var txRecord = tranList.FirstOrDefault(p => p.InStock == txViewModel.Num);

            var except = "Success";
            var actual = transactionRepository.AddTransactionLog(txViewModel);
            Assert.AreEqual(except, actual);

            CheckReceived(product, txViewModel, dbAsset, txRecord);
        }

        /// <summary>
        /// 賣出部分零股
        /// </summary>
        [TestMethod()]
        public void AddTransactionLog_SoldOddLot()
        {
            //已有該產品的零股紀錄
            dbConnection.QueryableTransactionRecords.ReturnsForAnyArgs(
                new List<TransactionRecord>() { FakeTransactionRecord.StockProduct1_TX_3 }.AsQueryable());

            var txViewModel = SetTransactionViewModel(true, product, 100, 20, 23, 0, false);
            var tranList = dbConnection.QueryableTransactionRecords.OrderBy(p => p.TransactionTime).Where(p =>
                p.ProductSeq == txViewModel.ProductSeq && p.InStock > 0);
            var txRecord = tranList.Any(p => p.InStock < 1000 && p.InStock> txViewModel.Num)
                ? tranList.FirstOrDefault(p => p.InStock < 1000 && p.InStock > txViewModel.Num)
                : tranList.FirstOrDefault(p => p.InStock == 1000);


            var except = "Success";
            var actual = transactionRepository.AddTransactionLog(txViewModel);
            Assert.AreEqual(except, actual);

            CheckReceived(product, txViewModel, dbAsset, txRecord);
        }

        /// <summary>
        /// 賣出部分零股且有合併
        /// </summary>
        [TestMethod()]
        public void AddTransactionLog_SoldOddLotWithMarge()
        {
            //已有該產品的零股紀錄
            dbConnection.QueryableTransactionRecords.ReturnsForAnyArgs(
                new List<TransactionRecord>()
                {
                    FakeTransactionRecord.StockProduct1_TX_2,
                    FakeTransactionRecord.StockProduct1_TX_3,
                }.AsQueryable());

            var txViewModel = SetTransactionViewModel(true, product, 600, 20, 23, 0, false);
            var tranList = dbConnection.QueryableTransactionRecords.OrderBy(p => p.TransactionTime).Where(p =>
                p.ProductSeq == txViewModel.ProductSeq && p.InStock > 0);
            var txRecord = tranList.Any(p => p.InStock < 1000)
                ? tranList.FirstOrDefault(p => p.InStock < 1000)
                : tranList.FirstOrDefault();


            var except = "Success";
            var actual = transactionRepository.AddTransactionLog(txViewModel);
            Assert.AreEqual(except, actual);

            CheckReceived(product, txViewModel, dbAsset, txRecord);
        }

        #endregion 賣出


        #region Tool

        /// <summary>
        /// 建立測試需要的交易紀錄
        /// </summary>
        /// <param name="soldStatus">是否賣出</param>
        /// <param name="product">交易產品</param>
        /// <param name="txNum">交易數量</param>
        /// <param name="txCashDividends">交易現金股利</param>
        /// <param name="txUnitPrice">交易單價</param>
        /// <param name="txFee">交易手續費</param>
        /// <param name="isDividends">配股交易</param>
        /// <returns></returns>
        private static TransactionViewModel SetTransactionViewModel(bool soldStatus, Product product, int txNum,
             int txUnitPrice, int txFee, int txCashDividends, bool isDividends)
        {
            var txViewModel = new TransactionViewModel()
            {
                SoldStatus = soldStatus,
                ProductSeq = product.ProductSeq,
                Num = txNum,
                CashDividends = txCashDividends,
                UnitPrice = txUnitPrice,
                Fee = txFee,
                isDividends = isDividends,
                TransactionTime = DateTime.Now,
            };
            return txViewModel;
        }

        private void CheckReceived(Product product, TransactionViewModel txViewModel, Asset dbAsset, TransactionRecord txRecord = null)
        {
            dbConnection.Received().QueryableTransactionRecords.FirstOrDefault(p => p.ProductSeq == txViewModel.ProductSeq);
            if (txViewModel.SoldStatus)
            {
                CheckSoldReceived(product, txViewModel, txRecord);
            }
            else
            {
                CheckBuyReceived(product, txViewModel, txRecord);
            }


            CheckAssetReceived(product, txViewModel, dbAsset, txRecord);
            //存檔
            dbConnection.Received().SaveChanges();
        }

        /// <summary>
        /// 判斷買入流程
        /// 如為零股的話就更新現有零股資料累加上去
        /// 如零股累加後能湊成一張，就更新成一張再新增一筆零股資料
        /// </summary>
        /// <param name="product"></param>
        /// <param name="txViewModel">本次的交易資料</param>
        /// <param name="txRecord">傳入的現有零股資料供計算，零股交易才會傳值</param>
        private void CheckBuyReceived(Product product, TransactionViewModel txViewModel, TransactionRecord txRecord)
        {
            var updateOddLot = txViewModel.Num < 1000 && txRecord != null;
            var updateNum = updateOddLot ? txRecord.Num + txViewModel.Num : txViewModel.Num;

            //判斷零股是否可湊成一張
            var totalOddLotNum = updateOddLot ? txViewModel.Num + txRecord.InStock : 0;
            if (updateOddLot && totalOddLotNum > 1000)
            {
                //拆成兩筆Transaction紀錄
                var remainderNum = totalOddLotNum % 1000;
                updateNum = txViewModel.Num - remainderNum;
                dbConnection.Received().Modified(Arg.Is<TransactionRecord>(p =>
                        p.ProductSeq == product.ProductSeq && p.Num == remainderNum),
                    EntityState.Added);
                dbConnection.Received().Modified(Arg.Is<TransactionRecord>(p =>
                        p.ProductSeq == product.ProductSeq && p.Num == updateNum + txRecord.Num),
                    EntityState.Modified);
            }
            else
            {
                //判斷買入經過的資料
                dbConnection.Received().Modified(Arg.Is<TransactionRecord>(p =>
                        p.ProductSeq == product.ProductSeq && p.Num == updateNum),
                    updateOddLot ? EntityState.Modified : EntityState.Added);
            }


            //判斷沒有經過賣出的資料
            dbConnection.DidNotReceive()
                .Modified(Arg.Is<Bookkeeping>(p => p.ProductSeq == txViewModel.ProductSeq), EntityState.Added);
        }

        /// <summary>
        /// 確認賣出時經過的資料
        /// </summary>
        /// <param name="product"></param>
        /// <param name="txViewModel"></param>
        /// <param name="txRecord">賣出時更新的資料</param>
        private void CheckSoldReceived(Product product, TransactionViewModel txViewModel, TransactionRecord txRecord)
        {
            var tranList = dbConnection.Received().QueryableTransactionRecords.OrderBy(p => p.TransactionTime).ThenByDescending(p => p.UnitPrice)
                .Where(p => p.ProductSeq == txViewModel.ProductSeq && p.InStock > 0);
            var oddTransaction = tranList.FirstOrDefault(p => p.InStock < 1000);

            if (oddTransaction != null && oddTransaction.InStock < txViewModel.Num)
            {
                txRecord.Num += oddTransaction.InStock;
                dbConnection.Modified(Arg.Is<TransactionRecord>(p=>p.Seq==oddTransaction.Seq && p.InStock==0), EntityState.Modified);
            }

            var updateStock = txRecord.Num - txViewModel.Num;
            dbConnection.Received()
                .Modified(Arg.Is<TransactionRecord>(p => p.ProductSeq == product.ProductSeq && p.InStock == updateStock),
                    EntityState.Modified);
            dbConnection.Received()
                .Modified(Arg.Is<Bookkeeping>(p => p.ProductSeq == txViewModel.ProductSeq), EntityState.Added);

            //判斷沒有經過買入的資料
            dbConnection.Modified(Arg.Is<TransactionRecord>(p => p.ProductSeq == product.ProductSeq), EntityState.Added);
            dbConnection.DidNotReceive()
                .Modified(Arg.Is<TransactionRecord>(p => p.ProductSeq == product.ProductSeq && p.Num == txViewModel.Num),
                    EntityState.Added);
        }

        /// <summary>
        /// 確認資產計算的流程
        /// </summary>
        /// <param name="product">產品</param>
        /// <param name="txViewModel">本次交易</param>
        /// <param name="dbAsset">資產的資料</param>
        /// <param name="txRecord">賣出時判斷成本</param>
        private void CheckAssetReceived(Product product, TransactionViewModel txViewModel, Asset dbAsset,
            TransactionRecord txRecord)
        {
            var countNum = txViewModel.SoldStatus ? 0 - txViewModel.Num : txViewModel.Num;
            var cost = txViewModel.SoldStatus ? 0 - txRecord.TotalPrice : txViewModel.Num * txViewModel.UnitPrice;

            var stockDividends = txViewModel.isDividends
                ? dbAsset.StockDividends + txViewModel.Num
                : dbAsset.StockDividends;
            var cashDividends = txViewModel.isDividends
                ? dbAsset.CashDividends + txViewModel.CashDividends
                : dbAsset.CashDividends;

            //計算資產
            dbConnection.Received().QueryableAssets.FirstOrDefault(p => p.ProductSeq == txViewModel.ProductSeq);
            dbConnection.Received().Modified(Arg.Is<Asset>(p =>
                p.ProductSeq == product.ProductSeq
                && p.Num == dbAsset.Num + countNum
                && p.TotalPrice == dbAsset.TotalPrice + cost
                && p.StockDividends == stockDividends
                && p.CashDividends == cashDividends
            ), EntityState.Modified);
        }

        #endregion Tool

    }
}