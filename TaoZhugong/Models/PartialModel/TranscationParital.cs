using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaoZhugong.Models.DbEntities
{
    [MetadataType(typeof(TransactionMetadata))]
    public partial class TransactionRecord
    {
        public class TransactionMetadata
        {
            [DisplayName("序號")]
            public int Seq { get; set; }
            [DisplayName("產品序號")]
            public int ProductSeq { get; set; }

            [DisplayName("數量")]
            public int Num { get; set; }
            [DisplayName("單價")]
            public double UnitPrice { get; set; }
            [DisplayName("總價")]
            public double TotalPrice { get; set; }
            [DisplayName("售價")]
            public Nullable<double> SalePrice { get; set; }

            [DisplayName("買入手續費")]
            public double AdministractionFee { get; set; }

            [DisplayName("賣出手續費&證交稅")]
            public Nullable<int> SaleTax { get; set; }
            [DisplayName("交易時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}",
                ApplyFormatInEditMode = true)]
            public System.DateTime TransactionTime { get; set; }
            [DisplayName("建立時間")]
            public System.DateTime CreateTime { get; set; }
            [DisplayName("備註")]
            public string Remark { get; set; }
            [DisplayName("賣出時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}",
                ApplyFormatInEditMode = true)]
            public Nullable<System.DateTime> SaleTime { get; set; }

            [DisplayName("庫存")]
            public int InStock { get; set; }


        }
        [DisplayName("產品名稱")]
        public string ProductName { get; set; }

        [DisplayName("投資報酬率")]
        public string ROI { get; set; }

        [DisplayName("損益")]
        public int Incomes { get; set; }

    }

}

