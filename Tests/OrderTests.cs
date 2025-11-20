using System;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class OrderTests
    {
        private string filePath;

        [SetUp]
        public void Setup()
        {
            filePath = Path.Combine(Path.GetTempPath(), "orders_test.xml");
            if (File.Exists(filePath))
                File.Delete(filePath);
            Order.ClearExtent();
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        [Test]
        public void Constructor_ValidData_ShouldCreateOrder()
        {
            var order = new Order("1234-5678-9012-3456");

            Assert.That(order.CardInfo, Is.EqualTo("1234-5678-9012-3456"));
           
            Assert.That(Order.Orders.Count, Is.EqualTo(1));
        }

        [Test]
        public void Constructor_EmptyCardInfo_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Order(""));
        }

        [Test]
        public void ToString_ShouldIncludeCardInfoAndDate()
        {
            var order = new Order("5555-4444-3333-2222");
            string output = order.ToString();

            Assert.That(output, Does.Contain("Card Info"));
            Assert.That(output, Does.Contain("Order made on"));
        }

        [Test]
        public void SaveAndLoad_ShouldPersistOrders()
        {
            var o1 = new Order("9876-5432-1111-2222");
            var o2 = new Order("1234-5678-9999-8888");

            Order.Save(filePath);
            Order.ClearExtent();
            Order.Load(filePath);

            Assert.That(Order.Orders.Count, Is.EqualTo(2));
            Assert.That(Order.Orders[0].CardInfo, Is.EqualTo("9876-5432-1111-2222"));
            Assert.That(Order.Orders[1].CardInfo, Is.EqualTo("1234-5678-9999-8888"));
        }
    }
}
