using System;
using System.Web.Mvc;
using CakeExchange.Models;

namespace CakeExchange.Controllers
{
    public class HomeController : Controller
    {
        private readonly OrderHundler orderHundler;
        private readonly object dblock;

        public HomeController()
        {
            orderHundler = new OrderHundler();
            dblock = new object();
        }

        // GET: Home
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Order order, string action)
        {
            if (ModelState.IsValid)
            {
                lock (dblock)
                {
                    order.Type = action == "Buy" ? OrderType.Buy : OrderType.Sell;
                    order.AdditionTime = DateTime.Now;
                    orderHundler.HundleNewOrder(order);
                }
            }

            return View();
        }
    }
}