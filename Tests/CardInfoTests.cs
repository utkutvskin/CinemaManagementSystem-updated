using System;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class CardInfoTests
    {
        //Test Name
        [Test]
        public void NameSetter_ShouldThrowException()
        {
            var card = new CardInfo("John Doe", "1234567890121111", DateTime.Now.AddYears(1), "123");
            Assert.Throws<ArgumentException>(() => card.Name = " ");
        }

        [Test]
        public void NameSetter_ShouldSetSuccessfully()
        {
            var card = new CardInfo("John Doe", "1234567890121111", DateTime.Now.AddYears(1), "123");
            card.Name = "Jane Smith";
            Assert.That(card.Name, Is.EqualTo("Jane Smith"));
        }

        //Test Number
        [Test]
        public void NumberSetter_ShouldThrowExceptionWhenNotAllDigits()
        {
            var card = new CardInfo("John Doe", "1234567890121111", DateTime.Now.AddYears(1), "123");
            Assert.Throws<ArgumentException>(() => card.Number = "1234567890o21111");
        }

        [Test]
        public void NumberSetter_ShouldThrowExceptionWhenTooShort()
        {
            var card = new CardInfo("John Doe", "1234567890121111", DateTime.Now.AddYears(1), "123");
            Assert.Throws<ArgumentException>(() => card.Number = "12");
        }

        [Test]
        public void NumberSetter_ShouldSetSuccessfully()
        {
            var card = new CardInfo("John Doe", "1234567890121111", DateTime.Now.AddYears(1), "123");
            card.Number = "0987654321234567";
            Assert.That(card.Number, Is.EqualTo("0987654321234567"));
        }

        // ExpiryDate
        [Test]
        public void ExpiryDateSetter_ShouldThrowException()
        {
            var card = new CardInfo("John Doe", "1234567890121111", DateTime.Now.AddMonths(1), "123");
            Assert.Throws<ArgumentException>(() => card.ExpiryDate = DateTime.Now.AddMonths(-2));
        }

        [Test]
        public void ExpiryDateSetter_ShouldSetSuccessfully()
        {
            var card = new CardInfo("John Doe", "1234567890121111", DateTime.Now.AddYears(1), "123");
            var future = DateTime.Now.AddYears(2);
            card.ExpiryDate = future;
            Assert.That(card.ExpiryDate, Is.EqualTo(future));
        }

        // PIN
        [Test]
        public void PinSetter_ShouldThrowExceptionWhenNot3()
        {
            var card = new CardInfo("John Doe", "1234567890121111", DateTime.Now.AddYears(1), "123");
            Assert.Throws<ArgumentException>(() => card.PINcode = "12345");
        }
        
        [Test]
        public void PinSetter_ShouldThrowExceptionWhenNotAllDigits()
        {
            var card = new CardInfo("John Doe", "1234567890121111", DateTime.Now.AddYears(1), "123");
            Assert.Throws<ArgumentException>(() => card.PINcode = "12a");
        }
        
        [Test]
        public void PinSetter_ShouldSetSuccessfully()
        {
            var card = new CardInfo("John Doe", "1234567890121111", DateTime.Now.AddYears(1), "123");
            card.PINcode = "999";
            Assert.That(card.PINcode, Is.EqualTo("999"));
        }

        // Constructor
        [Test]
        public void Constructor_ValidData_ShouldCreateCard()
        {
            var future = DateTime.Now.AddYears(1);
            var card = new CardInfo("John Doe", "1234567890121111", future, "123");

            Assert.That(card.Name, Is.EqualTo("John Doe"));
            Assert.That(card.Number, Is.EqualTo("1234567890121111"));
            Assert.That(card.ExpiryDate, Is.EqualTo(future));
            Assert.That(card.PINcode, Is.EqualTo("123"));
        }

        // ToString
        [Test]
        public void ToString_ShouldContain_Name_Number_Expiry()
        {
            var future = new DateTime(2030, 5, 1);
            var card = new CardInfo("John Doe", "1234567890121111", future, "123");
            var text = card.ToString();

            StringAssert.Contains("John Doe", text);
            StringAssert.Contains("123456", text);
            StringAssert.Contains("05/2030", text);
        }
    }
}
