using System;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class SeatTests
    {
        private string filePath = "seats_test.xml";

        [SetUp]
        public void SetUp()
        {
            // Delete temp XML file before each test
            if (File.Exists(filePath))
                File.Delete(filePath);

            // Clear static list
            Seat.ClearAllSeats();
        }

        [Test]
        public void Constructor_ValidSeat_ShouldCreateSeat()
        {
            var seat = new Seat(5, 'B');
            Assert.AreEqual(5, seat.Number);
            Assert.AreEqual('B', seat.Row);
        }

        [Test]
        public void Constructor_DuplicateSeat_ShouldThrowException()
        {
            new Seat(1, 'A');
            Assert.Throws<ArgumentException>(() => new Seat(1, 'A'));
        }

        [Test]
        public void Constructor_InvalidNumber_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Seat(0, 'A'));
        }

        [Test]
        public void Constructor_InvalidRow_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Seat(5, '1'));
        }

        [Test]
        public void SaveAndLoad_ShouldPersistSeats()
        {
            // Arrange
            new Seat(1, 'A');
            new Seat(2, 'B');
            new Seat(3, 'C');

            Seat.Save(filePath);

            // Clear list for loading test
            Seat.ClearAllSeats();

            // Act
            Seat.Load(filePath);

            // Assert
            var seats = Seat.Seats;
            Assert.AreEqual(3, seats.Count);
            Assert.AreEqual(1, seats[0].Number);
            Assert.AreEqual('A', seats[0].Row);
        }
    }
}