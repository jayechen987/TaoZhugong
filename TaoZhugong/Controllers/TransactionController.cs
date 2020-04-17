using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaoZhugong.Models.DbEntities;
using TaoZhugong.Models.Transaction;
using TaoZhugong.Models.ViewModel;
using TaoZhugong.Models.WebProfile;

namespace TaoZhugong.Controllers
{
    public class TransactionController : Controller
    {
        ITransactionRepository transactionRepository;
        private DDLRepository ddlRepository = new DDLRepository();
        public TransactionController()
        {
            transactionRepository = new TransactionRepository();
        }

        // GET: Transcation
        public ActionResult AddTransaction(string type, bool soldStatus, int productSeq = 0)
        {
            ViewBag.ProductDDL = ddlRepository.GetProductDDLByType(type, productSeq);
            var model = transactionRepository.GetAddTransactionView(soldStatus, productSeq);
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult AddTransaction(TransactionViewModel transaction)
        {
            var result = "false";
            if (ModelState.IsValid)
            {
                result = transactionRepository.AddTransactionLog(transaction);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int productSeq)
        {
            var model = transactionRepository.GetTransactionListByProduct(productSeq);
            return View(model);
        }

        public ActionResult AddDividends(int productSeq)
        {
            return PartialView(new Dividends() { ProductSeq = productSeq });
        }

        [HttpPost]
        public JsonResult AddDividends(Dividends dividends)
        {
            var result = "false";
            if (ModelState.IsValid)
            {
                result = transactionRepository.AddDividends(dividends);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFee(int totalPrice,bool isSale)
        {
            var fee = transactionRepository.GetFee(totalPrice,isSale);
            return Json(fee, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDividends()
        {
            transactionRepository.DividendSchedule();

            return Json("123", JsonRequestBehavior.AllowGet);
        }
    }
}