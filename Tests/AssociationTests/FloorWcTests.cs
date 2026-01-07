using CinemaManagementSystem.Area;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Exceptions;
using NUnit.Framework;

namespace CinemaManagementSystem.Tests.AssociationTests;

[TestFixture]
public class FloorWcTests
{
    [SetUp]
    public void SetUp()
    {
        Floor.ClearExtent();
    }
    
    [Test]
    public void CreateWcWithFloor_ShouldCreateWc_WhenUseWcConstructor()
    {
        Floor floor = new Floor(1);
        
        var wc = new WC(WCTypeEnum.Male, floor);
        
        Assert.That(wc.Floor, Is.EqualTo(floor));
        Assert.That(floor.WCs.Count, Is.EqualTo(1));
        
        
    }

    
    [Test]
    public void CreateWcWithFloor_ShouldThrowException_WhenWcIsExists()
    {
        Floor floor = new Floor(1);
        
        var wc = new WC(WCTypeEnum.Male, floor);
        
        Assert.Throws<DuplicateException>(() => new WC(WCTypeEnum.Male, floor));
    }
    
    
    [Test]
    public void AddWcToFloor_ShouldAddWc_WhenUseMethodInFloorClass()
    {
        Floor floor = new Floor(1);
        floor.AddWC(WCTypeEnum.Male);
        floor.AddWC(WCTypeEnum.Female);
        
        Assert.That(floor.WCs.Count, Is.EqualTo(2));
    }
    
    [Test]
    public void RemoveWC_ShouldRemove()
    {
        Floor floor = new Floor(1);
        
        var wc1 = new WC(WCTypeEnum.Male, floor);
        
        var wc2 = new WC(WCTypeEnum.Female, floor);

        floor.RemoveWc(wc2);
        
        Assert.That(floor.WCs.Count, Is.EqualTo(1));
    }
}