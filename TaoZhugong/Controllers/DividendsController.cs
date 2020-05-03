using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaoZhugong.Models.DbEntities;
using TaoZhugong.Models.Dividend;

namespace TaoZhugong.Controllers
{
    public class DividendsController : Controller
    {
        IDividendsRepository dividendsRepository = new DividendsRepository();
        // GET: Dividends
        public ActionResult Index()
        {
            var list = dividendsRepository.GetDividendList();
            return View(list);
        }
    }
}