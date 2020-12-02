using OnlineStore.DAL;
using OnlineStore.Models;
using OnlineStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Controllers
{
    public class ProductDashboardController : Controller
    {
        // GET: ProductDashboard
        public ActionResult Index()
        {
            var dbContext = new OnlineStoreContext();
            var productDashboardViewModelList = new List<ProductDashboardViewModel>();

            var productDashboard = (from product in dbContext.Product select product);

            foreach (var item in productDashboard)
            {
                var productItem = new ProductDashboardViewModel();
                productItem.ProductId = item.ProductId;
                productItem.Name = item.Name;
                productItem.Description = item.Description;
                productItem.ImageURL = item.ImageURL;
                productItem.ListPrice = (from product in dbContext.ProductVendor where product.ProductId == item.ProductId select product.ListPrice).FirstOrDefault();

                productDashboardViewModelList.Add(productItem);
            }
            
            return View(productDashboardViewModelList);
        }

        public int CartCheckout(string cartData)
        {
            Console.WriteLine(cartData);
            return 1;
        }
    }
}