using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaoZhugong.Models;

namespace TaoZhugong.Controllers
{
    public class ProductController : Controller
    {
        IProductRepository productRepository;

        public ProductController()
        {
            productRepository = new ProductRepository();
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
    }
}