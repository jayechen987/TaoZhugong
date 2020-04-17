using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Schema;
using TaoZhugong.Models.CustomerException;
using TaoZhugong.Models.DbEntities;
using TaoZhugong.Models.ViewModel;
using TaoZhugong.Models.WebProfile.Enum;

namespace TaoZhugong.Models.Transaction
{
    public class TransactionRepository : ITransactionRepository
    {
        ITaoZhugongDatabaseConnection dbConnection;

        public TransactionRepository()
        {
            dbConnection = new TaoZhugongDatabaseConnection();
        }

        public TransactionRepository(ITaoZhugongDatabaseConnection _dbConnection)
        {
            dbConnection = _dbConnection;

        }

        #region Transaction
        /// <summary>
        /// 預設交易紀錄的ViewModel
        /// </summary>
        /// <param name="soldStatus"></param>
        /// <param name="productSeq"></param>
        /// <returns></returns>
        public TransactionViewModel GetAddTransactionView(bool soldStatus, int productSeq)
        {
            return new TransactionViewModel()
            {
                ProductSeq = productSeq,
                SoldStatus = soldStatus,
                isDividends = false,
                Num = 1000,
            };
        }

        /// <summary>
        /// 新增交易紀錄(含資產計算)
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public string AddTransactionLog(TransactionViewModel transaction)
        {
            //本次交易數量
            int transNum = 0;
            //本次成本
            int cost = 0;
            //如手續費為0且非配股交易則自動帶入
            transaction.Fee = transaction.Fee == 0 & !transaction.isDividends ? 
                GetFee((int)(transaction.Num * transaction.UnitPrice), transaction.SoldStatus) 
                : transaction.Fee;

            //買入
            if (!transaction.SoldStatus)
            {
                Tx_BuyFunction(transaction, out transNum, out cost);
            }
            else
            {
                Tx_SaleFunction(transaction, out transNum, out cost);
            }

            try
            {
                RecalculateAsset(transaction, transNum, cost);

                dbConnection.SaveChanges();
                return "Success";
            }
            catch (Exception ee)
            {
                return ee.Message;
            }

        }

        /// <summary>
        /// 取產品的所有交易紀錄(含投資報酬率數據)
        /// </summary>
        /// <param name="productSeq"></param>
        /// <returns></returns>
        public List<TransactionRecord> GetTransactionListByProduct(int productSeq)
        {
            var productName = GetProductNameBySeq(productSeq);
            var bookkeepingList = dbConnection.QueryableBookkeepings.Where(p => p.ProductSeq == productSeq);
            return dbConnection.QueryableTransactionRecords.Where(p => p.ProductSeq == productSeq).ToList().Select(p =>
            {
                p.ProductName = productName;
                p.Incomes = bookkeepingList.Any(q => q.RelatedSeq == p.Seq) ? bookkeepingList.FirstOrDefault(q => q.RelatedSeq == p.Seq).Amount : 0;
                p.ROI = p.SalePrice == null
                    ? "0%"
                    : p.UnitPrice == 0 ? "--"
                    : Math.Round(
                        (Convert.ToDouble(p.Incomes) /
                         Convert.ToDouble(p.TotalPrice + p.AdministractionFee + p.SaleTax.Value)) * 100, 2) + "%";
                return p;
            }).ToList();
        }

        #endregion Transaction


        #region Dividends

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
            AddTransactionLog(transaction);

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

        #endregion Dividends

        /// <summary>
        /// 手續費:買進金額*手續費(0.1425%)*折扣
        /// 稅:買進金額*交易稅(0.3%)
        /// </summary>
        /// <returns></returns>
        public int GetFee(int totalPrice, bool isSale)
        {  
            var discount = 0.65;
            var tax = (int)Math.Round(totalPrice * 0.003);
            var fee = (int)Math.Round(totalPrice * 0.001425 * discount);


            return isSale ? fee + tax : fee;
        }

        #region Unit

        public string GetProductNameBySeq(int productSeq)
        {
            return dbConnection.QueryableProducts.FirstOrDefault(p => p.ProductSeq == productSeq)?.ProductName;
        }

        /// <summary>
        /// 交易紀錄_賣出
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="transNum">交易數量(負數)</param>
        /// <param name="transCost">交易成本(負數)</param>
        public void Tx_SaleFunction(TransactionViewModel transaction, out int transNum, out int transCost)
        {
            var tranList = dbConnection.QueryableTransactionRecords.OrderBy(p => p.TransactionTime).ThenByDescending(p => p.UnitPrice)
                .Where(p => p.ProductSeq == transaction.ProductSeq && p.InStock > 0);
            var updateTransaction = new TransactionRecord();

            var oddTransaction = tranList.FirstOrDefault(p => p.InStock < 1000);

            //除了零股足夠外其餘都直接取整張做交易
            updateTransaction = tranList.FirstOrDefault(p => p.InStock == 1000);

            if (transaction.Num < 1000)
            {
                //確認資產是否有零股且零股足夠交易
                if (oddTransaction != null && oddTransaction.InStock >= transaction.Num)
                {
                    updateTransaction = oddTransaction;
                }
                else if (oddTransaction != null)
                {
                    //不夠的話賣出整張後把餘額合併到原本的零股
                    updateTransaction.InStock += oddTransaction.InStock;
                    updateTransaction.TotalPrice += oddTransaction.InStock * oddTransaction.UnitPrice;
                    updateTransaction.UnitPrice = updateTransaction.TotalPrice / updateTransaction.InStock;

                    //清除舊有的零股
                    oddTransaction.InStock = 0;
                    oddTransaction.Remark += $"零股賣出合併至編號:{updateTransaction.Seq}";
                    dbConnection.Modified(oddTransaction, EntityState.Modified);
                }
            }


            updateTransaction.SaleTime = transaction.TransactionTime;
            updateTransaction.SalePrice = transaction.UnitPrice;
            updateTransaction.SaleTax = transaction.Fee;
            updateTransaction.InStock -= transaction.Num;
            updateTransaction.Remark += $"/ {transaction.Remark}";

            dbConnection.Modified(updateTransaction, EntityState.Modified);

            transNum = 0 - transaction.Num;
            //扣的資產應為交易本身的成本而非售價(此cost為負數)
            transCost = 0 - (int)(updateTransaction.UnitPrice * transaction.Num);

            //計算本次收益到記帳本
            var salePrice = (transaction.Num * transaction.UnitPrice);
            var incom = salePrice + transCost - transaction.Fee - updateTransaction.AdministractionFee;


            var newBookkepping = new Bookkeeping()
            {
                ProductSeq = transaction.ProductSeq,
                Type = BookkeepingType.TransactionRecord.ToString(),
                RelatedSeq = updateTransaction.Seq,
                Amount = (int)incom,
                CreateTime = DateTime.Now,
            };
            dbConnection.Modified(newBookkepping, EntityState.Added);
        }



        /// <summary>
        /// 交易紀錄_買入
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="transNum">交易數量</param>
        /// <param name="transCost">成本</param>
        public void Tx_BuyFunction(TransactionViewModel transaction, out int transNum, out int transCost)
        {
            var oddLot = dbConnection.QueryableTransactionRecords.FirstOrDefault(p =>
                p.ProductSeq == transaction.ProductSeq && p.InStock < 1000);
            transNum = transaction.Num;
            transCost = (int)(transaction.Num * transaction.UnitPrice);

            //判斷為零股交易且已有零股庫存時走更新流程
            if (transNum < 1000 && oddLot != null)
            {
                var updateCost = transCost;
                var updateNum = transNum;
                //判斷累加是否成一張
                CheckIsOverLotCase(transaction, transNum, oddLot, ref updateNum, ref updateCost);

                UpdateTransactionRecord(transaction, oddLot, updateCost, updateNum);
            }
            else
            {
                AddNewTransactionByViewModel(transaction);
            }

        }

        /// <summary>
        /// 根據交易重新計算資產
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="transNum"></param>
        /// <param name="transCost"></param>
        public void RecalculateAsset(TransactionViewModel transaction, int transNum, int transCost)
        {
            var asset = dbConnection.QueryableAssets.FirstOrDefault(p => p.ProductSeq == transaction.ProductSeq);

            asset.Num += transNum;
            asset.TotalPrice += transCost;
            asset.StockDividends += transaction.isDividends ? transNum : 0;
            asset.CashDividends += transaction.isDividends ? transaction.CashDividends : 0;
            asset.UpdateTime = DateTime.Now;
            //股票賣光時重置累積股息
            if (asset.Num == 0)
            {
                asset.CashDividends = 0;
                asset.StockDividends = 0;
            }

            dbConnection.Modified(asset, EntityState.Modified);
        }

        /// <summary>
        /// 根據交易VM更新Transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="oddLot"></param>
        /// <param name="updateCost"></param>
        /// <param name="updateNum"></param>
        private void UpdateTransactionRecord(TransactionViewModel transaction, TransactionRecord oddLot
            , int updateCost, int updateNum)
        {
            oddLot.UnitPrice = (oddLot.Num * oddLot.UnitPrice + updateCost) / (oddLot.Num + updateNum);
            oddLot.Num += updateNum;
            oddLot.InStock += updateNum;
            oddLot.TotalPrice += updateCost;
            oddLot.AdministractionFee += transaction.Fee;
            oddLot.TransactionTime = transaction.TransactionTime;
            oddLot.Remark += $" / {transaction.Remark}";
            dbConnection.Modified(oddLot, EntityState.Modified);
        }

        /// <summary>
        /// 確認新增的零股是否會湊成整張
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="transNum"></param>
        /// <param name="oddLot"></param>
        /// <param name="updateNum"></param>
        /// <param name="updateCost"></param>
        private void CheckIsOverLotCase(TransactionViewModel transaction, int transNum,
            TransactionRecord oddLot, ref int updateNum, ref int updateCost)
        {
            if (oddLot.InStock + transNum > 1000)
            {
                var remainderNum = (oddLot.InStock + transNum) % 1000;
                updateNum = transNum - remainderNum;
                updateCost = updateNum * (int)transaction.UnitPrice;
                //餘數股票作新增
                AddNewTransactionByViewModel(new TransactionViewModel()
                {
                    ProductSeq = transaction.ProductSeq,
                    UnitPrice = transaction.UnitPrice,
                    Num = remainderNum,
                    Fee = 0,
                    TransactionTime = transaction.TransactionTime,
                    Remark = transaction.Remark,
                });


            }
        }

        /// <summary>
        /// 根據ViewModel新增Transaction
        /// </summary>
        /// <param name="transaction"></param>
        private void AddNewTransactionByViewModel(TransactionViewModel transaction)
        {
            var newTrasn = new TransactionRecord()
            {
                ProductSeq = transaction.ProductSeq,
                UnitPrice = transaction.UnitPrice,
                Num = transaction.Num,
                InStock = transaction.Num,
                TotalPrice = transaction.Num * transaction.UnitPrice,
                AdministractionFee = transaction.Fee,
                TransactionTime = transaction.TransactionTime,
                CreateTime = DateTime.Now,
                Remark = transaction.Remark,
            };
            dbConnection.Modified(newTrasn, EntityState.Added);
        }

        #endregion Unit
    }
}