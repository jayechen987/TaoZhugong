using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using TaoZhugong.Models.DbEntities;

namespace TaoZhugong.Models.ViewModel
{
    public class DividendViewModel:Dividends
    {

        [DisplayName("產品名稱")]
        public string ProductName { get; set; }
        [DisplayName("已實現損益")]
        public int RealizedCashDividends { get; set; }
        [DisplayName("平均配息")]
        public double AverageCashDividends { get; set; }
        [DisplayName("平均配股")]
        public double AverageStockDividends { get; set; }
    }
}