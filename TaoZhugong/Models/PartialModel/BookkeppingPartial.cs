using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaoZhugong.Models.DbEntities
{
    [MetadataType(typeof(BookkeppingMetaData))]
    public partial class Bookkeeping
    {
        public class BookkeppingMetaData
        {
            [DisplayName("序號")]
            public int Seq { get; set; }
            [DisplayName("產品序號")]
            public int ProductSeq { get; set; }
            [DisplayName("記帳類型")]
            public string Type { get; set; }
            [DisplayName("關聯序號")]
            public int RelatedSeq { get; set; }
            [DisplayName("金額")]
            public int Amount { get; set; }
            [DisplayName("建立時間")]
            public System.DateTime CreateTime { get; set; }
        }
    }
}