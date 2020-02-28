using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaoZhugong.Models.DbEntities
{
    [MetadataType(typeof(ProductPartial))]
    public partial class Product
    {
        public class ProductPartial
        {
            [DisplayName("產品編號")]
            public int ProductSeq { get; set; }
            [Required]
            [DisplayName("類型")]
            public string Type { get; set; }
            [Required]
            [DisplayName("產品名稱")]
            public string ProductName { get; set; }
            [DisplayName("產品內容")]
            public string ProductValue { get; set; }
            [DisplayName("戶頭")]
            public string Owner { get; set; }
            [DisplayName("註記")]
            public string Remark { get; set; }
        }
    }
}