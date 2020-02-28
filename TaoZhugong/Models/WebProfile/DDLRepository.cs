using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaoZhugong.Models.WebProfile
{
    public static class DDLRepository
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
    }
}