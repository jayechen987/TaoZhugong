using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaoZhugong.Models;
using TaoZhugong.Models.DbEntities;
using TaoZhugong.Models.WebProfile;

namespace TaoZhugong.Controllers
{
    public class ProductController : Controller
    {
        IProductRepository productRepository;
        
        public ProductController()
        {
            productRepository = new ProductRepository();
            ViewBag.Owner = DDLRepository.GetProductOwnerDDL();
            ViewBag.TypeDDL = DDLRepository.GetProductTypeDDL();
        }

        public ProductController(IProductRepository _product)
        {
            productRepository = _product;
        }
    

        // GET: Product
        public ActionResult List()
        {
            var model = productRepository.GetProductList();
            return View(model);
        }

        public ActionResult Edit(int productId)
        {
            var editModel = productRepository.GetProductList().FirstOrDefault(p => p.ProductSeq == productId);
            return PartialView(editModel);
        }

        public JsonResult Edit(Product product)
        {
            string result = "false";
            if (ModelState.IsValid)
            {
                result = productRepository.EditProduct(product);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}