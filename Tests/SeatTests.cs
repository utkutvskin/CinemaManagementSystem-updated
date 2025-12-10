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
            Seat.ClearAllSeatsForTesting();
        }

        [Test]
        public void NumberSetterValidation_ShouldThrowException()
        {
            var seat = new Seat(5, 'B');

            Assert.Throws<ArgumentException>(() =>
                seat.Number = 0
            );
        }

        [Test]
        public void NumberSetterValidation_ShouldSetNumberSuccessfully()
        {
            var seat = new Seat(5, 'B');

            seat.Number = 10;

            Assert.That(seat.Number, Is.EqualTo(10));
        }

        //  Row setter
        [Test]
        public void RowSetterValidation_ShouldThrowException()
        {
            var seat = new Seat(5, 'B');

            Assert.Throws<ArgumentException>(() =>
                seat.Row = '1'
            );

        }

        [Test]
        public void RowSetterValidation_ShouldSetRowSuccessfully()
        {
            var seat = new Seat(5, 'B');

            seat.Row = 'c';

            Assert.That(seat.Row, Is.EqualTo('C'));
        }
        
        
        
        
        [Test]
        public void Constructor_ValidSeat_ShouldCreateSeat()
        {
            var seat = new Seat(5, 'B');
            Assert.AreEqual(5, seat.Number);
            Assert.AreEqual('B', seat.Row);
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
            
            if (File.Exists(filePath)) 
                File.Delete(filePath);
            
            new Seat(1, 'A');
            new Seat(2, 'B');
            new Seat(3, 'C');

            Seat.Save(filePath);

            Seat.ClearAllSeatsForTesting();

            Seat.Load(filePath);

            var seats = Seat.Seats;
            
            Assert.That(seats.Count, Is.EqualTo(3));
            Assert.That(seats[0].Number, Is.EqualTo(1));
            Assert.That(seats[0].Row, Is.EqualTo('A'));
        }
    }
}