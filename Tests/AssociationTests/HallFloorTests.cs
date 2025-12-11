using CinemaManagementSystem.Exceptions;
using NUnit.Framework;

namespace CinemaManagementSystem.Tests.AssociationTests;

[TestFixture]
public class HallFloorTests
{
    [SetUp]
    public void SetUp()
    {
        Floor.ClearExtent();
    }


    [Test]
    public void CreateHallWithFloor_ShouldCreateHall_WhenUseHallConstructor()
    {
        Floor floor = new Floor(1);
        
        var hall = new Hall(10, floor);
        
        Assert.That(hall.Floor, Is.EqualTo(floor));
        Assert.That(floor.Halls.Count, Is.EqualTo(1));
    }

    [Test]
    public void CreateHallWithFloor_ShouldThrowException_WhenHallIsExists()
    {
        Floor floor = new Floor(1);
        
        var hall = new Hall(10, floor);
        
        Assert.Throws<DuplicateException>(() => new Hall(10, floor));
    }
    
    [Test]
    public void AddHallToFloor_ShouldAddHall_WhenUseMethodInFloorClass()
    {
        Floor floor = new Floor(1);
        floor.AddHall(10);
        floor.AddHall(11);
        
        Assert.That(floor.Halls.Count, Is.EqualTo(2));
    }
    
    [Test]
    public void RemoveHall_ShouldRemove()
    {
        Floor floor = new Floor(1);
        
        var hall1 = new Hall(10, floor);
        
        var hall2 = new Hall(11, floor);

        floor.RemoveHall(hall1);
        
        Assert.That(floor.Halls.Count, Is.EqualTo(1));
    }
    

    
}