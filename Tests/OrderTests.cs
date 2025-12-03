using System;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class OrderTests
    {
        private string filePath = "orders_test.xml";

        [SetUp]
        public void Setup()
        {
            Order.ClearExtent();
        }
        
        //Test CardInfo setter
        [Test]
        public void CardInfoSetter_ShouldThrowException()
        {
            var card = new CardInfo(
                name: "John Doe",
                number: "1234567890123456",
                expiryDate: DateTime.Now.AddYears(1),
                pinCode: "123"
            );
            var order = new Order(card);
            
            Assert.Throws<ArgumentException>(() => order.cardInfo = null); 
        }
        [Test]
        public void CardInfoSetter_ShouldSetSuccessfully()
        {
            var card = new CardInfo(
                name: "John Doe",
                number: "1234567890123456",
                expiryDate: DateTime.Now.AddYears(1),
                pinCode: "123"
            );
            var order = new Order(card);
            
            var card1 = new CardInfo(
                name: "John Doe",
                number: "1234567890123456",
                expiryDate: DateTime.Now.AddYears(1),
                pinCode: "456"
            );
            
            order.cardInfo = card1;
            
            Assert.That(order.cardInfo.PINcode, Is.EqualTo("456"));
        }

        [Test]
        public void Constructor_ValidData_ShouldCreateOrder()
        {
            var card = new CardInfo(
                name: "John Doe",
                number: "1234567890123456",
                expiryDate: DateTime.Now.AddYears(1),
                pinCode: "123"
            );
            var order = new Order(card);

            Assert.That(order.cardInfo.Number, Is.EqualTo("1234567890123456"));
           
            Assert.That(Order.Orders.Count, Is.EqualTo(1));
        }

        [Test]
        public void Constructor_EmptyCardInfo_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Order(null));
        }

        [Test]
        public void ToString_ShouldIncludeCardInfoAndDate()
        {
            var card = new CardInfo(
                name: "Alice Smith",
                number: "5555444433332222",
                expiryDate: DateTime.Now.AddYears(2),
                pinCode: "431"
            );
            var order = new Order(card);
            
            string output = order.ToString();

            Assert.That(output, Does.Contain("Order made on"));
            Assert.That(output, Does.Contain("Card Holder"));
            Assert.That(output, Does.Contain("Alice Smith"));
        }

        [Test]
        public void SaveAndLoad_ShouldPersistOrders()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            
            var card1 = new CardInfo(
                name: "User One",
                number: "9876543211112222",
                expiryDate: DateTime.Now.AddYears(1),
                pinCode: "111"
            );
            var card2 = new CardInfo(
                name: "User Two",
                number: "1234567899998888",
                expiryDate: DateTime.Now.AddYears(1),
                pinCode: "222"
            );
            new Order(card1);
            new Order(card2);

            Order.Save(filePath);
            Order.ClearExtent();
            Order.Load(filePath);

            Assert.That(Order.Orders.Count, Is.EqualTo(2));
            Assert.That(Order.Orders[0].cardInfo.Number, Is.EqualTo("9876543211112222"));
            Assert.That(Order.Orders[1].cardInfo.Number, Is.EqualTo("1234567899998888"));
        }
    }
}
