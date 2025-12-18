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
            Item snack = new Snack("Popcorn", 50, 350);
            Item glasses = new Glass3D(30, isReusable: true);

            Assert.That(snack, Is.InstanceOf<Snack>());
            Assert.That(glasses, Is.InstanceOf<Glass3D>());
        }

        [Test]
        public void Item_BaseProperties_ShouldBeSet_ByDerivedConstructors()
        {
            var snack = new Snack("Nachos", 40, 500);
            var glasses = new Glass3D(25, isReusable: false);

            // Inherited properties from Item
            Assert.That(snack.Name, Is.EqualTo("Nachos"));
            Assert.That(snack.Price, Is.EqualTo(40));

            Assert.That(glasses.Name, Is.EqualTo("3D Glasses"));
            Assert.That(glasses.Price, Is.EqualTo(25));
        }

        [Test]
        public void Item_Validation_ShouldThrow_OnInvalidArguments()
        {
            // Item validation through derived constructors
            Assert.Throws<ArgumentException>(() => new Snack("", 10, 100));      // invalid name
            Assert.Throws<ArgumentException>(() => new Snack("Chips", -5, 100)); // invalid price
            Assert.Throws<ArgumentException>(() => new Snack("Chips", 10, 0));   // invalid calories

            Assert.Throws<ArgumentException>(() => new Glass3D(0, true));        // invalid price
        }
    }
}
