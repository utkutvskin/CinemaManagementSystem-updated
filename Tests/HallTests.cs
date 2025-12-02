using System;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class HallTests
    {
        private string _filePath;

        [SetUp]
        public void Setup()
        {
            _filePath = Path.Combine(Path.GetTempPath(), "halls_test_" + Guid.NewGuid() + ".xml");

            if (File.Exists(_filePath))
                File.Delete(_filePath);

            // use the new clear method to ensure no leftover state
            Hall.ClearAllHalls();
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

            Hall.ClearAllHalls();
        }

        [Test]
        public void Constructor_ValidNumber_ShouldCreateHall()
        {
            var hall = new Hall(1);
            Assert.That(hall.Number, Is.EqualTo(1));

            // constructor adds to static list
            Assert.That(Hall.Halls.Count, Is.EqualTo(1));
            Assert.That(Hall.Halls[0], Is.EqualTo(hall));
        }

        [Test]
        public void Constructor_InvalidNumber_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Hall(0));
            Assert.Throws<ArgumentException>(() => new Hall(-5));
        }

        [Test]
        public void Constructor_DuplicateNumber_ShouldThrowException()
        {
            Hall.ClearAllHalls();
            var h1 = new Hall(2);
            Assert.Throws<ArgumentException>(() => new Hall(2));
        }

        [Test]
        public void SaveAndLoad_ShouldPersistHalls()
        {
            // Arrange – create 3 halls
            Hall.ClearAllHalls();
            var h1 = new Hall(1);
            var h2 = new Hall(2);
            var h3 = new Hall(3);

            // Act – Save, clear in-memory, then load
            Hall.Save(_filePath);
            Hall.ClearAllHalls();
            Assert.That(Hall.Halls.Count, Is.EqualTo(0), "Expected in-memory halls to be cleared before Load.");

            Hall.Load(_filePath);

            // Assert – check loaded data
            Assert.That(Hall.Halls.Count, Is.EqualTo(3));
            Assert.That(Hall.Halls[0].Number, Is.EqualTo(1));
            Assert.That(Hall.Halls[1].Number, Is.EqualTo(2));
            Assert.That(Hall.Halls[2].Number, Is.EqualTo(3));
        }
    }
}
