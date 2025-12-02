using System;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class StampcardTests
    {
        private string _filePath;

        [SetUp]
        public void Setup()
        {
            _filePath = Path.Combine(Path.GetTempPath(), $"stampcards_test_{Guid.NewGuid()}.xml");

            // ensure clean static state
            Stampcard.ClearAllStampcards();

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

            Stampcard.ClearAllStampcards();
        }

        [Test]
        public void Constructor_InitializesCorrectly_AndAddedToExtent()
        {
            var card = new Stampcard();

            Assert.That(card.NumberOfStamps, Is.EqualTo(0));
            Assert.That(card.IsCompleted, Is.False);
            Assert.That(Stampcard.Stampcards.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddStamp_CompletesAtMaxAndThrowsOnFurtherAdd()
        {
            var card = new Stampcard();

            for (int i = 0; i < 10; i++)
                card.AddStamp();

            Assert.That(card.NumberOfStamps, Is.EqualTo(10));
            Assert.That(card.IsCompleted, Is.True);
            Assert.Throws<InvalidOperationException>(() => card.AddStamp());
        }

        [Test]
        public void InvalidPropertySets_ThrowArgumentException()
        {
            var card = new Stampcard();

            // DateOfPurchase cannot be set to future
            Assert.Throws<ArgumentException>(() => card.DateOfPurchase = DateTime.Now.AddDays(1));

            // NumberOfStamps cannot be negative or above max (implementation uses 10)
            Assert.Throws<ArgumentException>(() => card.NumberOfStamps = -1);
            Assert.Throws<ArgumentException>(() => card.NumberOfStamps = 11);
        }

        [Test]
        public void SaveAndLoad_PersistsStampcardsAndStampCounts()
        {
            var c1 = new Stampcard();
            var c2 = new Stampcard();
            c1.AddStamp();
            c2.AddStamp();
            c2.AddStamp();

            Stampcard.Save(_filePath);

            // Clear in-memory list to ensure Load restores data
            Stampcard.ClearAllStampcards();
            Assert.That(Stampcard.Stampcards.Count, Is.EqualTo(0));

            Stampcard.Load(_filePath);

            Assert.That(Stampcard.Stampcards.Count, Is.EqualTo(2));
            Assert.That(Stampcard.Stampcards[0].NumberOfStamps, Is.EqualTo(1));
            Assert.That(Stampcard.Stampcards[1].NumberOfStamps, Is.EqualTo(2));
        }
    }
}
