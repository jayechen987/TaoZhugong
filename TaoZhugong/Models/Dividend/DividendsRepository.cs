using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TaoZhugong.Models.CustomerException;
using TaoZhugong.Models.DbEntities;
using TaoZhugong.Models.Transaction;
using TaoZhugong.Models.ViewModel;

namespace TaoZhugong.Models.Dividend
{
    public class DividendsRepository : IDividendsRepository
    {
        private ITaoZhugongDatabaseConnection dbConnection;
        ITransactionRepository transactionRepository;

        public DividendsRepository()
        {
            dbConnection = new TaoZhugongDatabaseConnection();
            transactionRepository = new TransactionRepository();

        }

        public DividendsRepository(ITaoZhugongDatabaseConnection _dbConnection, ITransactionRepository _transactionRepository)
        {
            dbConnection = _dbConnection;
            transactionRepository = _transactionRepository;

        }

        /// <summary>
        /// 新增配股配息紀錄
        /// </summary>
        /// <param name="dividends"></param>
        /// <returns></returns>
        public string AddDividends(Dividends dividends)
        {
            dividends.CreateTime = DateTime.Now;

            dbConnection.Modified(dividends, EntityState.Added);
            dbConnection.SaveChanges();
            return "Success";
        }

        /// <summary>
        /// 配股日到後根據資料表對交易紀錄跟資產做加減
        /// </summary>
        /// <param name="dividends"></param>
        public void SetDividendsSchedule(Dividends dividends)
        {
            if (dividends.ProductSeq == 0)
            {
                throw new DataKeyIsNullException();
            }
            //todo 修改配股配息的條件為判斷賣出日
            var ownStuckList = dbConnection.QueryableTransactionRecords.Where(p =>
                p.ProductSeq == dividends.ProductSeq &&
                (p.SaleTime == null || p.SaleTime <= dividends.ExRightDate)
            ).ToList();

            if (!ownStuckList.Any())
            {
                return;
            }

            var addMoney = (int)(ownStuckList.Sum(p => p.Num) * dividends.CashDividends);
            var addStuck = (int)(ownStuckList.Sum(p => p.Num) * dividends.StockDividend * 0.1);

            //新增transaction
            var transaction = new TransactionViewModel()
            {
                SoldStatus = false,
                ProductSeq = dividends.ProductSeq,
                Num = addStuck,
                CashDividends = addMoney,
                UnitPrice = 0,
                Fee = 0,
                isDividends = true,
                Remark = $"{DateTime.Now.ToString("yyyy")} 股利，配股 {dividends.StockDividend} /配息 {dividends.CashDividends} 。",
                TransactionTime = DateTime.Now,
            };

            //新增交易紀錄
            transactionRepository.AddTransactionLog(transaction);

            dividends.TransactionRecordSeq = dbConnection.QueryableTransactionRecords.Max(p => p.Seq);
            dbConnection.SaveChanges();

        }

        public void DividendSchedule()
        {
            var today = DateTime.Now.Date;
            var runScheduleList =
                dbConnection.QueryableDividends.Where(p => p.TransactionRecordSeq == 0 && p.DividendDate <= today).ToList();
            foreach (var dividends in runScheduleList)
            {
                SetDividendsSchedule(dividends);
            }
        }
    }
}