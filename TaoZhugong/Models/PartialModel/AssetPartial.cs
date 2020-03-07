using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaoZhugong.Models.DbEntities
{

    [MetadataType(typeof(AssetMetadata))]
    public partial class Asset 
    {
        public class AssetMetadata
        {
            [DisplayName("序號")]
            public int Seq { get; set; }
            [DisplayName("產品序號")]
            public int ProductSeq { get; set; }



            [DisplayName("數量")]
            public int Num { get; set; }
            [DisplayName("總價")]
            public double TotalPrice { get; set; }

            [DisplayName("累積股利")]
            public int StockDividends { get; set; }
            [DisplayName("累積股息")]
            public int CashDividends { get; set; }

            [DisplayName("建立時間")]
            public System.DateTime CreateTime { get; set; }
            [DisplayName("更新時間")]
            public System.DateTime UpdateTime { get; set; }
        }

        [DisplayName("產品")]
        public string ProductName { get; set; }

        [DisplayName("均價")]
        public double AveragePrice { get; set; }

        [DisplayName("損益平衡點")]
        public double BreakevenPoint { get; set; }

    }
}