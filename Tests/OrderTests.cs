using System;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class OrderTests
    {
        private string _filePath;

        [SetUp]
        public void Setup()
        {
            // Use a unique temp file per test to avoid collisions
            _filePath = Path.Combine(Path.GetTempPath(), $"orders_test_{Guid.NewGuid()}.xml");

            // Ensure clean static state
            Order.ClearAllOrders();
            if (File.Exists(_filePath))
                File.Delete(_filePath);
        }

        [TearDown]
        public void Cleanup()
        {
            try
            {
                if (File.Exists(_filePath))
                    File.Delete(_filePath);
            }
            catch { /* ignore cleanup errors */ }

            Order.ClearAllOrders();
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
            Assert.Throws<ArgumentException>(() => new Order("   "));
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
            // Arrange
            var o1 = new Order("9876-5432-1111-2222");
            var o2 = new Order("1234-5678-9999-8888");

            // Act
            Order.Save(_filePath);

            // Clear in-memory list to ensure Load restores data
            Order.ClearAllOrders();
            Assert.That(Order.Orders.Count, Is.EqualTo(0));

            Order.Load(_filePath);

            // Assert
            Assert.That(Order.Orders.Count, Is.EqualTo(2));
            Assert.That(Order.Orders[0].CardInfo, Is.EqualTo("9876-5432-1111-2222"));
            Assert.That(Order.Orders[1].CardInfo, Is.EqualTo("1234-5678-9999-8888"));
        }
    }
}
