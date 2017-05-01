using System;
using System.Data.Entity.Validation;
using System.Linq;
using CakeExchange.Models;
using Effort;
using NUnit.Framework;

namespace CakeExchangeTests
{
    [TestFixture]
    public class Tests
    {
        private OrderContext context;
        private OrderHundler hundler;

        [SetUp]
        public void Initialize()
        {
            var connection = DbConnectionFactory.CreateTransient();
            context = new OrderContext(connection);
            hundler = new OrderHundler(context);
        }


        [TestCase(OrderType.Buy, TestName = "Add new BUY order")]
        [TestCase(OrderType.Sell, TestName = "Add new SELL order")]
        public void AddNewOrder(OrderType orderType)
        {
            var order = new Order(1, 1.0f, 1, "email@mail.ru", orderType, DateTime.Now);

            hundler.HundleNewOrder(order);

            Assert.Contains(order, context.Orders.ToList());
            Assert.AreEqual(1, context.Orders.Count());
        }

        [Test]
        public void Exchange_WithEqualPrices()
        {
            var sellOrder = new Order(1, 5.0f, 5, "sender@mail.ru", OrderType.Sell, DateTime.Now);
            var purchaseOrder = new Order(2, 5.0f, 5, "purchaser@mail.ru", OrderType.Buy, DateTime.Now);

            hundler.HundleNewOrder(sellOrder);
            hundler.HundleNewOrder(purchaseOrder);

            Assert.IsEmpty(context.Orders);
            Assert.AreEqual(1, context.History.Count());
        }

        [TestCase(OrderType.Buy, TestName = "New BUY order with residue")]
        [TestCase(OrderType.Sell, TestName = "New SELL order with residue")]
        public void EqualPrices_WithResidue(OrderType orderType)
        {
            var oppositeType = GetOppositeOrderType(orderType);
            var oldOrder = new Order(1, 5.0f, 5, "sender@mail.ru", oppositeType, DateTime.Now);
            hundler.HundleNewOrder(oldOrder);

            var newOrder = new Order(2, 5.0f, 10, "purchaser@mail.ru", orderType, DateTime.Now);
            hundler.HundleNewOrder(newOrder);

            Assert.AreEqual(1, context.Orders.Count());
            Assert.AreEqual(1, context.History.Count());
        }

        [TestCase(OrderType.Buy, TestName = "New BUY order with several exchanges")]
        [TestCase(OrderType.Sell, TestName = "New SELL order with several exchanges")]
        public void SeveralExchanges_WithEqualPrices(OrderType orderType)
        {
            var oppositeType = GetOppositeOrderType(orderType);
            for (int i = 1; i < 4; i++)
            {
                hundler.HundleNewOrder(new Order(i, 5.0f, 5, "email@mail.ru", oppositeType, DateTime.Now));
            }

            var newOrder = new Order(4, 5.0f, 15, "email@mail.ru", orderType, DateTime.Now);
            hundler.HundleNewOrder(newOrder);

            Assert.IsEmpty(context.Orders);
            Assert.AreEqual(3, context.History.Count());
        }

        [TestCase(OrderType.Buy, 10, 5, TestName = "BUY cheap order")]
        [TestCase(OrderType.Sell, 5, 10, TestName = "SELL expensive order")]
        public void Exchange_WithDifferentPrices(OrderType orderType, float newOrderPrice, float oldOrderPrice)
        {
            var oppositeType = GetOppositeOrderType(orderType);
            hundler.HundleNewOrder(new Order(1, oldOrderPrice, 5, "email1@mail.ru", oppositeType, DateTime.Now));

            var newOrder = new Order(2, newOrderPrice, 5, "email2@mail.ru", orderType, DateTime.Now);
            hundler.HundleNewOrder(newOrder);

            Assert.IsEmpty(context.Orders);
            Assert.AreEqual(1, context.History.Count());
            Assert.AreEqual(oldOrderPrice, context.History.Single().Price);
        }

        [TestCase(OrderType.Buy, 5, 10, TestName = "Don't buy expensive order")]
        [TestCase(OrderType.Sell, 10, 5, TestName = "Don't sell cheap order")]
        public void No_Exchange_WithDifferentPrices(OrderType orderType, float newOrderPrice, float oldOrderPrice)
        {
            var oppositeType = GetOppositeOrderType(orderType);
            hundler.HundleNewOrder(new Order(1, oldOrderPrice, 5, "email1@mail.ru", oppositeType, DateTime.Now));

            var newOrder = new Order(2, newOrderPrice, 5, "email2@mail.ru", orderType, DateTime.Now);
            hundler.HundleNewOrder(newOrder);

            Assert.AreEqual(2, context.Orders.Count());
            Assert.AreEqual(0, context.History.Count());
        }

        [TestCase(-5.0f, 5, "email@mail.ru", TestName = "Wrong price")]
        [TestCase(5.0f, -5, "email@mail.ru", TestName = "Wrong count")]
        [TestCase(5.0f, 5, "wrong email", TestName = "Wrong email")]
        public void NonValidFields(float price, int count, string email)
        {
            var wrongOrder = new Order(1, price, count, email, OrderType.Buy, DateTime.Now);

            TestDelegate action = () => hundler.HundleNewOrder(wrongOrder);

            Assert.Catch<DbEntityValidationException>(action);
        }

        private OrderType GetOppositeOrderType(OrderType orderType)
        {
            return orderType == OrderType.Buy ? OrderType.Sell : OrderType.Buy;
        }
    }
}
