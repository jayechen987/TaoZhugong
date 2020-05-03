using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaoZhugong.Models.DbEntities;

namespace TaoZhugong.Controllers
{
    public class DividendsController : Controller
    {

        // GET: Dividends
        public ActionResult Index()
        {
            var list = new List<Dividends>();
            return View(list);
        }
    }
}