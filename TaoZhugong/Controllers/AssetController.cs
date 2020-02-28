using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaoZhugong.Models;

namespace TaoZhugong.Controllers
{
    public class AssetController : Controller
    {
        AssetRepository _assetRepository = new AssetRepository();
        // GET: Asset
        public ActionResult List(string type = "Stock")
        {
            ViewBag.AssetType = type;
            var list = _assetRepository.GetAssetListByType(type);
            return View(list);
        }
    }
}