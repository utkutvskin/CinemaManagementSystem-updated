using System;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class HallTests
    {
        private string filePath;

        [SetUp]
        public void Setup()
        {
            filePath = Path.Combine(Path.GetTempPath(), "halls_test.xml");

            if (File.Exists(filePath))
                File.Delete(filePath);

            Hall.ClearExtent();
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            Hall.ClearExtent();
        }

        [Test]
        public void Constructor_ValidNumber_ShouldCreateHall()
        {
            var hall = new Hall(1);
            Assert.That(hall.Number, Is.EqualTo(1));
        }

        [Test]
        public void Constructor_InvalidNumber_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Hall(0));
        }

        [Test]
        public void Constructor_DuplicateNumber_ShouldThrowException()
        {
            new Hall(2);
            Assert.Throws<ArgumentException>(() => new Hall(2));
        }

        [Test]
        public void SaveAndLoad_ShouldPersistHalls()
        {
            // Arrange – 3 farklı salon oluştur
            var h1 = new Hall(1);
            var h2 = new Hall(2);
            var h3 = new Hall(3);

            // Act – Kaydet, temizle, sonra yükle
            Hall.Save(filePath);
            Hall.ClearExtent();
            Hall.Load(filePath);

            // Assert – 3 kayıt geldi mi kontrol et
            Assert.That(Hall.Halls.Count, Is.EqualTo(3));
            Assert.That(Hall.Halls[0].Number, Is.EqualTo(1));
            Assert.That(Hall.Halls[1].Number, Is.EqualTo(2));
            Assert.That(Hall.Halls[2].Number, Is.EqualTo(3));
        }
    }
}