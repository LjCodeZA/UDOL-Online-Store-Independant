using OnlineStore.DAL;
using OnlineStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Controllers
{
    public class CheckoutController : Controller
    {
        // GET: Checkout
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string cartData)
        {
            //Do something when the list is empty
            if (string.IsNullOrEmpty(cartData))
                return View();

            return View(ReconstructCartData(cartData));
        }

        [HttpPost]
        public ActionResult confirmation(string cartData)
        {
            var reconstructedCartData = ReconstructCartData(cartData);

            var dbContext = new OnlineStoreContext();

            foreach (var item in reconstructedCartData)
            {
                var newEntry = new Models.Order();
                newEntry.Quantity = item.Quantity;
                newEntry.CreatedDate = DateTime.Now;
                newEntry.Recon = null; //Not really necessary
                newEntry.ProductVendorId = GetProductVendorIdFromProductId(item.ProductId); //Select first vendor
                newEntry.TotalPrice = item.ListPrice;

                dbContext.Order.Add(newEntry);
            }

            dbContext.SaveChanges();
            return View(reconstructedCartData);
        }

        private int GetProductVendorIdFromProductId(int productId)
        {
            var dbContext = new OnlineStoreContext();

            return (from productVendor in dbContext.ProductVendor where productVendor.ProductId == productId select productVendor.ProductVendorId).FirstOrDefault();
        }

        [HttpGet]
        public ActionResult confirmation()
        {
            return View();
        }

        public IEnumerable<CartItemsViewModel> ReconstructCartData(string cartData)
        {
            var cartItemList = new List<CartItemsViewModel>();

            bool moreThan1Item = false;
            if (cartData.Contains("#"))
                moreThan1Item = true;
            else
                moreThan1Item = false;

            if (moreThan1Item)
            {
                var items = cartData.Split('#');

                for (int i = 0; i < items.Length; i++)
                {
                    var cartItem = new CartItemsViewModel();
                    var individualParts = items[i].Split('|');

                    var productId = int.Parse(individualParts[0]);
                    var quantity = int.Parse(individualParts[1]);
                    var productName = individualParts[2].ToString();

                    cartItem.ProductId = productId;
                    cartItem.Quantity = quantity;
                    cartItem.ProductName = productName;
                    cartItem.OriginalCartString = cartData;

                    var productListPrice = GetListPriceByProductId(productId);
                    cartItem.ListPrice = productListPrice;

                    cartItemList.Add(cartItem);
                }

            }
            else
            {
                var cartItem = new CartItemsViewModel();
                var individualParts = cartData.Split('|');

                var productId = int.Parse(individualParts[0]);
                var quantity = int.Parse(individualParts[1]);
                var productName = individualParts[2].ToString();

                cartItem.ProductId = productId;
                cartItem.Quantity = quantity;
                cartItem.ProductName = productName;
                cartItem.OriginalCartString = cartData;

                var productListPrice = GetListPriceByProductId(productId);
                cartItem.ListPrice = productListPrice;

                cartItemList.Add(cartItem);
            }

            var cartItemListConsolidated = 
            cartItemList
            .Select(x =>
               new CartItemsViewModel
               {
                   ProductId = x.ProductId,
                   ProductName = x.ProductName,
                   Quantity = x.Quantity,
                   OriginalCartString = x.OriginalCartString,
                   ListPrice = x.ListPrice
               }
             )
             .GroupBy(s => new { s.ProductId, s.ProductName, s.OriginalCartString })
             .Select(g =>
                   new CartItemsViewModel
                   {
                       ProductId = g.Key.ProductId,
                       ProductName = g.Key.ProductName,
                       Quantity = g.Sum(x => x.Quantity),
                       OriginalCartString = g.Key.OriginalCartString,
                       ListPrice = g.Sum(x => x.ListPrice * x.Quantity)
                   }
             );

            ViewBag.TotalPrice = cartItemListConsolidated.Sum(x => x.ListPrice);

            return cartItemListConsolidated;
        }

        private float GetListPriceByProductId(int productId)
        {
            var dbContext = new OnlineStoreContext();

            return (from product in dbContext.ProductVendor where product.ProductId == productId select product.ListPrice).FirstOrDefault();
        }
    }
}