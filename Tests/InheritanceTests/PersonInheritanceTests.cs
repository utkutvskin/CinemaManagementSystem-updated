using NUnit.Framework;
using CinemaManagementSystem.Person;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Person.Roles;
using System;

namespace CinemaManagementSystem.Tests.InheritanceTests
{
    [TestFixture]
    public class PersonInheritanceTests
    {
        [Test]
        public void StandardInheritance_CustomerAndEmployee_ArePeople()
        {
            // Creating instances of derived classes.
            var customer = new Customer("Ali", "Veli", GenderEnum.Male, new DateTime(1990, 1, 1));
            var employee = new Employee("Ayse", "Yilmaz", new DateTime(1995, 5, 5), GenderEnum.Female, Role.Manager);

            // Proving the 'IS-A' relationship. Both Customer and Employee are treated as 'Person'.
            Assert.IsInstanceOf<Person>(customer);
            Assert.IsInstanceOf<Person>(employee);
        }

        [Test]
        public void SharedAttributes_PersonAttributes_ShouldBeAccessible()
        {
            // Using a derived class (Customer) to access base class attributes.
            Person p1 = new Customer("Test", "User", GenderEnum.Female, DateTime.Now);
            
            // Proving Code Reuse. 'Name' is defined in 'Person' but used here.
            p1.Name = "New Name";

            Assert.AreEqual("New Name", p1.Name);
        }
    }
}
