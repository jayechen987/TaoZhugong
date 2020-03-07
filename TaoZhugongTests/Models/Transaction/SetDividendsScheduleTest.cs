using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaoZhugong.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using TaoZhugong.Models.CustomerException;
using TaoZhugong.Models.DbEntities;
using TaoZhugongTests.FakeData;

namespace TaoZhugong.Models.Tests
{
    [TestClass()]
    public class SetDividendsScheduleTest
    {
        ITaoZhugongDatabaseConnection dbConnection;
        ITransactionRepository transactionRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            dbConnection = Substitute.For<ITaoZhugongDatabaseConnection>();
            transactionRepository = new TransactionRepository(dbConnection);

            dbConnection.QueryableTransactionRecords.Returns(FakeTransactionRecord.transactionRecorList.AsQueryable());
            dbConnection.QueryableAssets.Returns(FakeAsset.AssetList.AsQueryable());
        }

        [TestMethod()]
        public void SetDividendsSchedule_DividendsKeyIsNull()
        {
            var addDividends = new Dividends();

            Action action = () =>
            {
                transactionRepository.SetDividendsSchedule(addDividends);
            };
            action.Should().Throw<DataKeyIsNullException>();


            dbConnection.DidNotReceive().QueryableTransactionRecords.Where(p =>
                p.ProductSeq == addDividends.ProductSeq && p.SalePrice == null &&
                p.TransactionTime <= addDividends.ExRightDate.Date);
            //確認沒有進入AddTransaction的重新計算資產的
            dbConnection.DidNotReceive().Modified(Arg.Is<TransactionRecord>(p => p.ProductSeq == addDividends.ProductSeq), EntityState.Added);
            dbConnection.DidNotReceive().QueryableAssets.FirstOrDefault(p => p.ProductSeq == addDividends.ProductSeq);
            dbConnection.DidNotReceive().SaveChanges();

        }

        [TestMethod()]
        public void SetDividendsSchedule_OwnStuckIsNull()
        {
            var addDividends = new Dividends() { ProductSeq = 99 };

            transactionRepository.SetDividendsSchedule(addDividends);


            dbConnection.Received().QueryableTransactionRecords.Where(p =>
                p.ProductSeq == addDividends.ProductSeq && p.SalePrice == null &&
                p.TransactionTime <= addDividends.ExRightDate.Date);
            //確認沒有進入AddTransaction的重新計算資產的
            dbConnection.DidNotReceive().Modified(Arg.Is<TransactionRecord>(p => p.ProductSeq == addDividends.ProductSeq), EntityState.Added);
            dbConnection.DidNotReceive().QueryableAssets.FirstOrDefault(p => p.ProductSeq == addDividends.ProductSeq);
            dbConnection.DidNotReceive().SaveChanges();

        }

        [TestMethod()]
        public void SetDividendsSchedule_addMoneyIsNull()
        {
            var addDividends = SetAddDividends(0, 0.5);

            transactionRepository.SetDividendsSchedule(addDividends);
           
            GetStuckDividendsNum(addDividends,out var addStuck,out var addMoney);

            CheckDividendsReceived(addDividends, addStuck, addMoney);
        }

        


        [TestMethod()]
        public void SetDividendsSchedule_addStockIsNull()
        {
            var addDividends = SetAddDividends(0.5, 0);
            GetStuckDividendsNum(addDividends, out var addStuck, out var addMoney);

            transactionRepository.SetDividendsSchedule(addDividends);

            CheckDividendsReceived(addDividends, addStuck, addMoney);

        }
        [TestMethod()]
        public void SetDividendsSchedule_Success()
        {
            var addDividends = SetAddDividends(0.5, 0.22);
            GetStuckDividendsNum(addDividends, out var addStuck, out var addMoney);

            transactionRepository.SetDividendsSchedule(addDividends);

            CheckDividendsReceived(addDividends, addStuck, addMoney);
        }

        private Dividends SetAddDividends(double cashDividends, double stockDividend)
        {
            var addDividends = new Dividends()
            {
                ProductSeq = FakeProduct.StockProduct1.ProductSeq,
                CashDividends = cashDividends,
                StockDividend = stockDividend,
                ExRightDate = DateTime.Now.AddDays(10),
                DividendDate = DateTime.Now.AddDays(40),
                CreateTime = DateTime.Now,
            };
            return addDividends;
        }

        private void GetStuckDividendsNum(Dividends addDividends,out int addStuck,out int addMoney)
        {
            var ownStuckList = FakeTransactionRecord.transactionRecorList.Where(p =>
                p.ProductSeq == addDividends.ProductSeq && p.SalePrice == null
                                                        && p.TransactionTime.Date <= addDividends.ExRightDate.Date);

            addMoney = (int)(ownStuckList.Sum(p => p.Num) * addDividends.CashDividends);
            addStuck = (int)(ownStuckList.Sum(p => p.Num) * addDividends.StockDividend * 0.1);
        }

        private void CheckDividendsReceived(Dividends addDividends, int addStuck, int addMoney)
        {
            dbConnection.Received().QueryableTransactionRecords.Where(p =>
                p.ProductSeq == addDividends.ProductSeq && p.SalePrice == null &&
                p.TransactionTime <= addDividends.ExRightDate.Date);
            dbConnection.Received().Modified(Arg.Is<TransactionRecord>(p =>
                p.ProductSeq == addDividends.ProductSeq
                && p.Num == addStuck
            ), EntityState.Added);
            dbConnection.Received().Modified(Arg.Is<Asset>(p =>
                p.ProductSeq == addDividends.ProductSeq
                && p.Num == FakeAsset.StockProduct1.Num + addStuck
                && p.StockDividends == addStuck
                && p.CashDividends == addMoney
            ), EntityState.Modified);
            dbConnection.Received().SaveChanges();
        }
    }
}