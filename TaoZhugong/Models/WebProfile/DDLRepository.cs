using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaoZhugong.Models.DbEntities;

namespace TaoZhugong.Models.WebProfile
{
    public class DDLRepository
    {
        public static List<SelectListItem> GetProductTypeDDL()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem(){ Text = "請選擇",Value = ""},
                new SelectListItem(){ Text = "股票",Value = "Stock"},
                new SelectListItem(){ Text = "外幣",Value = "Currency"}
            };
        }

        public static List<SelectListItem> GetProductOwnerDDL()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem(){ Text = "請選擇",Value = ""},
                new SelectListItem(){ Text = "群益證卷",Value = "群益"},
                new SelectListItem(){ Text = "富邦",Value = "富邦"},
                new SelectListItem(){ Text = "Richart",Value = "Richart"},
            };
        }

        public List<SelectListItem> GetProductDDLByType(string type, int productSeq)
        {
            TaoZhugongEntities db = new TaoZhugongEntities();

            var productList = db.Product.Where(p => p.Type == type).ToList();

            List<SelectListItem> returnList = new List<SelectListItem>()
            {
                new SelectListItem(){Text="--請選擇--",Value = ""}
            };

            returnList.AddRange(productList.Select(p =>
                new SelectListItem()
                {
                    Value = p.ProductSeq.ToString(),
                    Text = p.ProductName,
                    Selected = p.ProductSeq == productSeq
                }).ToList());

            return returnList;

        }
    }
}