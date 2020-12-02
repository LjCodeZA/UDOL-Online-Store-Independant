using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineStore.ViewModels
{
    public class CartItemsViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public float ListPrice { get; set; }
        public float TotalPrice { get; set; }
        public string OriginalCartString { get; set; }
    }
}