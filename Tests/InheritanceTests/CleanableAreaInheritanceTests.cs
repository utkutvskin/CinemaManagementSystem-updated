using NUnit.Framework;
using CinemaManagementSystem.Area;
using CinemaManagementSystem.Enums;
using System.Collections.Generic;

namespace CinemaManagementSystem.Tests.InheritanceTests
{
    [TestFixture]
    public class CleanableAreaInheritanceTests
    {
        [Test]
        public void Polymorphism_AllAreas_ShouldBeInSameList()
        {
            // Creating different concrete types to show Standard Inheritance.
            var hall = new Hall(1);
            var floor = new Floor(1);
            var wc = new WC(WCTypeEnum.Female, floor);

            // Proving Polymorphism. Storing Hall, Floor, and WC in a single 'CleanableArea' list.
            List<CleanableArea> cleaningList = new List<CleanableArea> { hall, floor, wc };

            // Verifying that the abstract list correctly holds derived types.
            Assert.AreEqual(3, cleaningList.Count);
            Assert.IsInstanceOf<Hall>(cleaningList[0]);
            Assert.IsInstanceOf<WC>(cleaningList[2]);
        }

        [Test]
        public void SharedLogic_IsNeedToBeCleaned_ShouldWorkForBaseClass()
        {
            // Upcasting derived class (Hall) to base class (CleanableArea).
            var hall = new Hall(2);
            CleanableArea area = hall; 

            // Proving Code Reuse. The logic in base class works for all children without rewriting.
            Assert.IsTrue(area.IsNeedToBeCleaned, "Base class logic should apply to derived types.");
        }
    }
}
