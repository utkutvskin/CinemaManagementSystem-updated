using System;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class SeatTests
    {
        private string _filePath;

        [SetUp]
        public void SetUp()
        {
            // unique temp file per test to avoid collisions
            _filePath = Path.Combine(Path.GetTempPath(), $"seats_test_{Guid.NewGuid()}.xml");

            // ensure clean static state
            Seat.ClearAllSeats();

            if (File.Exists(_filePath))
                File.Delete(_filePath);
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                if (File.Exists(_filePath))
                    File.Delete(_filePath);
            }
            catch { /* ignore cleanup errors */ }

            Seat.ClearAllSeats();
        }

        [Test]
        public void Constructor_ValidSeat_ShouldCreateSeat()
        {
            var seat = new Seat(5, 'B');
            Assert.AreEqual(5, seat.Number);
            Assert.AreEqual('B', seat.Row);

            // constructor adds to static list
            Assert.That(Seat.Seats.Count, Is.EqualTo(1));
            Assert.That(Seat.Seats[0], Is.EqualTo(seat));
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
            Assert.Throws<ArgumentException>(() => new Seat(-3, 'A'));
        }

        [Test]
        public void Constructor_InvalidRow_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Seat(5, '1'));
            Assert.Throws<ArgumentException>(() => new Seat(5, '@'));
        }

        [Test]
        public void Row_LowercaseIsNormalizedToUppercase()
        {
            var seat = new Seat(10, 'c');
            Assert.AreEqual('C', seat.Row);
        }

        [Test]
        public void SaveAndLoad_ShouldPersistSeats()
        {
            // Arrange
            Seat.ClearAllSeats();
            new Seat(1, 'A');
            new Seat(2, 'B');
            new Seat(3, 'C');

            // Act
            Seat.Save(_filePath);

            // Clear in-memory list to ensure Load restores data
            Seat.ClearAllSeats();
            Assert.AreEqual(0, Seat.Seats.Count);

            Seat.Load(_filePath);

            // Assert
            var seats = Seat.Seats;
            Assert.AreEqual(3, seats.Count);
            Assert.AreEqual(1, seats[0].Number);
            Assert.AreEqual('A', seats[0].Row);
            Assert.AreEqual(2, seats[1].Number);
            Assert.AreEqual('B', seats[1].Row);
            Assert.AreEqual(3, seats[2].Number);
            Assert.AreEqual('C', seats[2].Row);
        }

        [Test]
        public void Seats_ReadOnlyList_ShouldNotAllowModification()
        {
            Seat.ClearAllSeats();
            var s = new Seat(7, 'D');
            var list = Seat.Seats;
            Assert.Throws<NotSupportedException>(() => ((System.Collections.IList)list).Add(s));
        }
    }
}
