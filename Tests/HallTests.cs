using System;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class HallTests
    {
        private string filePath = "halls_test.xml";

        [SetUp]
        public void Setup()
        {
            Hall.ClearExtent();
        }

        //test hall number
        [Test]
        public void HallValidation_ShouldThrowException()
        {
            Hall hall = new Hall(10);
            Assert.Throws<ArgumentException>(() =>
                hall.Number = -9
            );
        }

        [Test]
        public void HallValidation_ShouldSetSuccessfully()
        {
            Hall hall = new Hall(10);
            hall.Number = 90;
            Assert.That(hall.Number, Is.EqualTo(90));
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
        public void SaveAndLoad_ShouldPersistHalls()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            var h1 = new Hall(1);
            var h2 = new Hall(2);
            var h3 = new Hall(3);

            Hall.Save(filePath);
            Hall.ClearExtent();
            Hall.Load(filePath);

            Assert.That(Hall.Halls.Count, Is.EqualTo(3));
            Assert.That(Hall.Halls[0].Number, Is.EqualTo(1));
            Assert.That(Hall.Halls[1].Number, Is.EqualTo(2));
            Assert.That(Hall.Halls[2].Number, Is.EqualTo(3));
        }
        
    }
}
