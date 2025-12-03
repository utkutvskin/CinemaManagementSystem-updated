using System;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class StampcardTests
    {
        private string filePath = "stampcards_test.xml";

        [SetUp]
        public void Setup()
        {
            Stampcard.ClearExtent();
        }

        [Test]
        public void DateOfPurchaseSetter_ShouldThrowException()
        {
            var card = new Stampcard();

            Assert.Throws<ArgumentException>(() =>
                card.DateOfPurchase = DateTime.Now.AddDays(1)
            );
        }

        [Test]
        public void DateOfPurchaseSetter_ShouldSetSuccessfully()
        {
            var card = new Stampcard();
            var pastDate = new DateTime(2020, 1, 1);

            card.DateOfPurchase = pastDate;

            Assert.That(card.DateOfPurchase, Is.EqualTo(pastDate));
        }
        
        
        [Test]
        public void NumberOfStampsSetter_ShouldThrowException()
        {
            var card = new Stampcard();

            Assert.Throws<ArgumentException>(() =>
                card.NumberOfStamps = 5
            );
        }
        
        [Test]
        public void NumberOfStampsSetter_ShouldSetSuccessfully()
        {
            var card = new Stampcard();

            card.NumberOfStamps = 3;

            Assert.That(card.NumberOfStamps, Is.EqualTo(3));
        }
        
        [Test]
        public void Constructor_ShouldInitializeWithZeroStamps()
        {
            var card = new Stampcard();

            Assert.That(card.NumberOfStamps, Is.EqualTo(0));
            Assert.That(card.IsCompleted, Is.False);
            Assert.That(Stampcard.Stampcards.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddStamp_ShouldIncreaseStampCount()
        {
            var card = new Stampcard();

            card.AddStamp();

            Assert.That(card.NumberOfStamps, Is.EqualTo(1));
            Assert.That(card.IsCompleted, Is.False);
        }

        [Test]
        public void AddStamp_ShouldCompleteCard_WhenMaxReached()
        {
            var card = new Stampcard();

            for (int i = 0; i < 4; i++)
                card.AddStamp();

            Assert.That(card.IsCompleted, Is.True);
            Assert.That(card.NumberOfStamps, Is.EqualTo(4));
        }

        [Test]
        public void AddStamp_ShouldThrow_WhenAlreadyCompleted()
        {
            var card = new Stampcard();
            for (int i = 0; i < 4; i++)
                card.AddStamp();

            Assert.Throws<InvalidOperationException>(() => card.AddStamp());
        }

        [Test]
        public void ToString_ShouldIncludeStampInfo()
        {
            var card = new Stampcard();
            card.AddStamp();

            string text = card.ToString();

            Assert.That(text, Does.Contain("Stamps"));
            Assert.That(text, Does.Contain("Completed"));
        }

        [Test]
        public void SaveAndLoad_ShouldPersistStampcards()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            
            var c1 = new Stampcard();
            var c2 = new Stampcard();
            c1.AddStamp();
            c2.AddStamp();
            c2.AddStamp();

            Stampcard.Save(filePath);
            Stampcard.ClearExtent();
            Stampcard.Load(filePath);

            Assert.That(Stampcard.Stampcards.Count, Is.EqualTo(2));
            Assert.That(Stampcard.Stampcards[0].NumberOfStamps, Is.EqualTo(1));
            Assert.That(Stampcard.Stampcards[1].NumberOfStamps, Is.EqualTo(2));
        }
    }
}
