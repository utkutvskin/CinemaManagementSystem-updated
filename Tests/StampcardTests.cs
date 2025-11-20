using System;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class StampcardTests
    {
        private string filePath;

        [SetUp]
        public void Setup()
        {
            filePath = Path.Combine(Path.GetTempPath(), "stampcards_test.xml");
            if (File.Exists(filePath))
                File.Delete(filePath);
            Stampcard.ClearExtent();
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
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

            for (int i = 0; i < 10; i++)
                card.AddStamp();

            Assert.That(card.IsCompleted, Is.True);
            Assert.That(card.NumberOfStamps, Is.EqualTo(10));
        }

        [Test]
        public void AddStamp_ShouldThrow_WhenAlreadyCompleted()
        {
            var card = new Stampcard();
            for (int i = 0; i < 10; i++)
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
