using System;
using NUnit.Framework;
using CinemaManagementSystem.Items;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class ItemInheritanceTests
    {
        [Test]
        public void Items_ShouldSupportPolymorphism_AsBaseType()
        {
            // Base type references holding different subclasses
            Item snack = new Snack("Popcorn", 50, 350, 20);
            Item glasses = new Glass3D("s", 30,20);

            Assert.That(snack, Is.InstanceOf<Snack>());
            Assert.That(glasses, Is.InstanceOf<Glass3D>());
        }

        [Test]
        public void Item_BaseProperties_ShouldBeSet_ByDerivedConstructors()
        {
            var snack = new Snack("Nachos", 40, 500, 20);
            var glasses = new Glass3D("m", 30,20);

            // Inherited properties from Item
            Assert.That(snack.Name, Is.EqualTo("Nachos"));
            Assert.That(snack.Price, Is.EqualTo(40));

            Assert.That(glasses.Price, Is.EqualTo(30));
        }

        [Test]
        public void Item_Validation_ShouldThrow_OnInvalidArguments()
        {
            // Item validation through derived constructors
            Assert.Throws<ArgumentException>(() => new Snack("", 10, 100, 30));      // invalid name
            Assert.Throws<ArgumentException>(() => new Snack("Chips", -5, 100, 20)); // invalid price
            Assert.Throws<ArgumentException>(() => new Snack("Chips", 10, -2, 30));   // invalid calories

            Assert.Throws<ArgumentException>(() => new Glass3D("S", -2, 23));        // invalid price
        }
    }
}
