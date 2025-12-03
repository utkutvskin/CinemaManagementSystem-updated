using System;
using System.Collections.Generic;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class DisplayerTests
    {
        [SetUp]
        public void Setup()
        {
          
            Hall.ClearExtent();
            Employee.ClearAllEmployees(); 
        }

        [Test]
        public void AddHall_ShouldUpdateBidirectionalRelationship()
        {
            var displayer = new Displayer("Ahmet", "Yilmaz", new DateTime(1990, 1, 1), DateTime.Now, 5000);
            var hall = new Hall(1);

            displayer.AddHall(hall);

            Assert.That(displayer.ManagedHalls.Count, Is.EqualTo(1));
            Assert.That(displayer.ManagedHalls, Does.Contain(hall));
            Assert.That(displayer.NumberOfScreensManaged, Is.EqualTo(1));

            Assert.That(hall.ManagedBy, Is.EqualTo(displayer));
        }

        [Test]
        public void RemoveHall_ShouldBreakRelationship()
        {
            var displayer = new Displayer("Ahmet", "Yilmaz", new DateTime(1990, 1, 1), DateTime.Now, 5000);
            var hall = new Hall(1);
            displayer.AddHall(hall);

            displayer.RemoveHall(hall);
            
            Assert.That(displayer.ManagedHalls.Count, Is.EqualTo(0));
            Assert.That(hall.ManagedBy, Is.Null);
        }

        [Test]
        public void AddHall_WhenAlreadyManaged_ShouldThrowException()
        {
            var displayer1 = new Displayer("Ahmet", "Yilmaz", new DateTime(1990, 1, 1), DateTime.Now, 5000);
            var displayer2 = new Displayer("Mehmet", "Kaya", new DateTime(1992, 1, 1), DateTime.Now, 5000);
            var hall = new Hall(1);

            displayer1.AddHall(hall);

            Assert.Throws<InvalidOperationException>(() =>
                displayer2.AddHall(hall)
            );
        }

        [Test]
        public void ManageSelectedScreens_ShouldAddMultipleHalls()
        {
            var displayer = new Displayer("Ahmet", "Yilmaz", new DateTime(1990, 1, 1), DateTime.Now, 5000);
            var hall1 = new Hall(1);
            var hall2 = new Hall(2);
            var halls = new List<Hall> { hall1, hall2 };

            displayer.ManageSelectedScreens(halls);

            Assert.That(displayer.NumberOfScreensManaged, Is.EqualTo(2));
            Assert.That(hall1.ManagedBy, Is.EqualTo(displayer));
            Assert.That(hall2.ManagedBy, Is.EqualTo(displayer));
        }

        [Test]
        public void DeleteHall_ShouldRemoveFromDisplayerList()
        {
          
            var displayer = new Displayer("Ahmet", "Yilmaz", new DateTime(1990, 1, 1), DateTime.Now, 5000);
            var hall = new Hall(5);
            
            displayer.AddHall(hall);
            
            Assert.That(displayer.NumberOfScreensManaged, Is.EqualTo(1));

            hall.DeleteHall();

            Assert.That(displayer.NumberOfScreensManaged, Is.EqualTo(0));
            Assert.That(displayer.ManagedHalls.Count, Is.EqualTo(0));
        }
    }
}
