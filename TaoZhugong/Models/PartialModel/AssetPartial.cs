using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaoZhugong.Models.d
{

    [MetadataType(typeof(AssetPartial))]
    public partial class Asset 
    { 
        public  class AssetPartial
        {
            public int Seq { get; set; }
            public int ProductSeq { get; set; }
            public int Num { get; set; }
            public double TotalPrice { get; set; }
            public int StockDividends { get; set; }
            public int CashDividends { get; set; }
            public System.DateTime CreatTime { get; set; }
            public System.DateTime UpdateTime { get; set; }
        }

    }
}