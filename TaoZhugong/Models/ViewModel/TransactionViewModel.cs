using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TaoZhugong.Models.ViewModel
{
    public class TransactionViewModel
    {
        public int ProductSeq { get; set; }
        [DisplayName("產品名稱")]
        public string ProductName { get; set; }
        [DisplayName("賣出")]
        public bool SoldStatus { get; set; }
        [DisplayName("數量")]
        public int Num { get; set; }
        [DisplayName("單價")]
        public double UnitPrice { get; set; }
        [DisplayName("手續費")]
        public int Fee { get; set; }
        [DisplayName("交易時間")]
        public System.DateTime TransactionTime { get; set; }
        [DisplayName("備註")]
        public string Remark { get; set; }

        [DisplayName("股利交易")]
        public bool isDividends { get; set; }
        [DisplayName("本次配息")]
        public int CashDividends { get; set; }
    }
}