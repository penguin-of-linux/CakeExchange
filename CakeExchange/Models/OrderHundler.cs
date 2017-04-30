using System;
using System.Linq;

namespace CakeExchange.Models
{
    public class OrderHundler
    {
        private readonly OrderContext context;

        public OrderHundler() : this(new OrderContext()) { }

        public OrderHundler(OrderContext context)
        {
            this.context = context;
        }

        public void HundleNewOrder(Order newOrder)
        {
            var oppositeOrderType = newOrder.Type == OrderType.Buy ? OrderType.Sell : OrderType.Buy;
            var orders = context.Orders.Where(o => o.Type == oppositeOrderType);
            orders = newOrder.Type == OrderType.Buy ? orders.OrderBy(o => o.Price) : orders.OrderByDescending(o => o.Price);
                

            foreach (var order in orders)
            {
                if (newOrder.Type == OrderType.Buy && order.Price > newOrder.Price) break;
                if (newOrder.Type == OrderType.Sell && order.Price < newOrder.Price) break;
                if (newOrder.Count == 0) break;

                var historyUnit = MakeOrderExchange(newOrder, order);
                context.History.Add(historyUnit);

                if (order.Count == 0) context.Orders.Remove(order);
            }

            if (newOrder.Count != 0) context.Orders.Add(newOrder);
            context.SaveChanges();
        }

        private HistoryUnit MakeOrderExchange(Order newOrder, Order order)
        {
            var countExchange = newOrder.Count < order.Count ? newOrder.Count : order.Count;
            newOrder.Count -= countExchange;
            order.Count -= countExchange;

            var purchaseOrder = new[] { newOrder, order }.Single(o => o.Type == OrderType.Buy);
            var sellerOrder = new[] { newOrder, order }.Single(o => o.Type == OrderType.Sell);

            return new HistoryUnit
            {
                LeadTime = DateTime.Now,
                PurchaseTime = purchaseOrder.AdditionTime,
                SaleTime = sellerOrder.AdditionTime,
                Price = order.Price,
                Count = countExchange,
                PurchaserEmail = purchaseOrder.Email,
                SellerEmail = sellerOrder.Email
            };
        }
    }
}