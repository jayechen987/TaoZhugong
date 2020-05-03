using System.Collections.Generic;
using TaoZhugong.Models.DbEntities;
using TaoZhugong.Models.ViewModel;

namespace TaoZhugong.Models.Transaction
{
    public interface ITransactionRepository
    {
        /// <summary>
        /// 預設交易紀錄的ViewModel
        /// </summary>
        /// <param name="soldStatus"></param>
        /// <param name="productSeq"></param>
        /// <returns></returns>
        TransactionViewModel GetAddTransactionView(bool soldStatus, int productSeq);

        /// <summary>
        /// 新增交易紀錄(含資產計算)
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        string AddTransactionLog(TransactionViewModel transaction);

        /// <summary>
        /// 取產品的所有交易紀錄(含投資報酬率數據)
        /// </summary>
        /// <param name="productSeq"></param>
        /// <returns></returns>
        List<TransactionRecord> GetTransactionListByProduct(int productSeq);

        string GetProductNameBySeq(int productSeq);

        /// <summary>
        /// 交易紀錄_賣出
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="transNum">交易數量(負數)</param>
        /// <param name="cost">交易成本(負數)</param>
        void Tx_SaleFunction(TransactionViewModel transaction, out int transNum, out int transCost);

        /// <summary>
        /// 交易紀錄_買入
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="transNum">交易數量</param>
        /// <param name="transCost">成本</param>
        void Tx_BuyFunction(TransactionViewModel transaction, out int transNum, out int transCost);

        void RecalculateAsset(TransactionViewModel transaction, int transNum, int transCost);

        /// <summary>
        /// 手續費:買進金額*手續費(0.1425%)*折扣
        /// 稅:買進金額*交易稅(0.3%)
        /// </summary>
        /// <returns></returns>
        int GetFee(int totalPrice, bool isSale);
    }
}
