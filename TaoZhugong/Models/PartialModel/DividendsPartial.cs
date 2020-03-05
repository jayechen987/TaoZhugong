using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaoZhugong.Models.DbEntities
{
    [MetadataType(typeof(DividendsMetaData))]
    public partial class Dividends
    {
        public class DividendsMetaData
        {
            [DisplayName("序號")]
            public int Seq { get; set; }
            [DisplayName("產品序號")]
            public int ProductSeq { get; set; }
            [DisplayName("除權除息日")]
            public System.DateTime ExRightDate { get; set; }
            [DisplayName("配股")]
            public double StockDividend { get; set; }
            [DisplayName("配息")]
            public double CashDividends { get; set; }
            [DisplayName("配股發放日")]
            public System.DateTime DividendDate { get; set; }
            [DisplayName("建立日期")]
            public System.DateTime CreateTime { get; set; }
            [DisplayName("關聯交易單號")]
            public int TransactionRecordSeq { get; set; }
            [DisplayName("除權息前股價")]
            public Nullable<double> StockPrice { get; set; }
        }
        [DisplayName("產品名稱")]
        public string ProductName { get; set; }
    }
}