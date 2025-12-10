using CinemaManagementSystem.Exceptions;
using NUnit.Framework;

namespace CinemaManagementSystem.Tests.AssociationTests;

[TestFixture]
public class HallSeatTests
{
    [SetUp]
    public void SetUp()
    {
        Hall.ClearExtent();
    }


    [Test]
    public void CreateSeatWithHall_ShouldCreateSeat_WhenUseSeatConstructor()
    {
        Hall hall = new Hall(10);
        
        var seat = new Seat(5, 'B', hall);
        
        Assert.That(seat.Hall, Is.EqualTo(hall));
        Assert.That(hall.Seats.Count, Is.EqualTo(1));
    }

    [Test]
    public void CreateSeatWithHall_ShouldThrowException_WhenSeatIsExists()
    {
        Hall hall = new Hall(10);
        var seat = new Seat(5, 'B', hall);
        
        Assert.Throws<DuplicateException>(() => new Seat(5, 'B', hall));
    }
    
    [Test]
    public void AddSeatToHall_ShouldAddSeat_WhenUseMethodInHallClass()
    {
        Hall hall = new Hall(10);
        hall.AddSeat(5, 'B');
        
        Assert.That(Seat.Seats.First(s => s.Number == 5 && s.Row=='B').Hall, Is.EqualTo(hall));
        Assert.That(hall.Seats.Count, Is.EqualTo(1));
    }
    
    [Test]
    public void RemoveSeat_ShouldRemove()
    {
        Hall hall = new Hall(10);
        Seat seat1 = hall.AddSeat(5, 'B');
        
        Seat seat2 = hall.AddSeat(5, 'C');

        hall.RemoveSeat(seat1);
        
        Assert.That(hall.Seats.Count, Is.EqualTo(1));
    }
    
    [Test]
    public void CreateSeatWithHall_ShouldThrowException_WhenSeatExceedsMaxCapacity()
    {
        Hall hall = new Hall(10);
        var seat = new Seat(5, 'A', hall);
        var seat2 = new Seat(5, 'B', hall);
        var seat3 = new Seat(5, 'C', hall);
        
        Assert.Throws<CapacityException>(() => new Seat(5, 'D', hall));
    }
    
    
}