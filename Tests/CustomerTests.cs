using System;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class CustomerTests
    {
        private string filePath = "customers_test.xml";

        [SetUp]
        public void SetUp()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            Customer.ClearAllCustomers();
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(filePath))
            {
                try { File.Delete(filePath); }
                catch { /* ignore */ }
            }

            Customer.ClearAllCustomers();
        }

        [Test]
        public void Constructor_ValidData_ShouldCreateCustomer()
        {
            var customer = new Customer("Alice", "Smith", "Female", new DateTime(1995, 3, 15));

            Assert.AreEqual("Alice", customer.Name);
            Assert.AreEqual("Smith", customer.Surname);
            Assert.AreEqual("Female", customer.Gender);
            Assert.AreEqual(1995, customer.BirthDate.Year);
        }

        [Test]
        public void Constructor_EmptyName_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Customer("", "Smith", "Female", new DateTime(1995, 3, 15))
            );
        }

        [Test]
        public void Constructor_EmptySurname_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Customer("Alice", "", "Female", new DateTime(1995, 3, 15))
            );
        }

        [Test]
        public void Constructor_FutureBirthDate_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Customer("Alice", "Smith", "Female", DateTime.Now.AddYears(1))
            );
        }

        [Test]
        public void Age_ShouldCalculateCorrectly()
        {
            var birthDate = new DateTime(2000, 6, 15);
            var customer = new Customer("Bob", "Jones", "Male", birthDate);

            int expectedAge = DateTime.Now.Year - 2000;
            if (DateTime.Now.DayOfYear < birthDate.DayOfYear)
                expectedAge--;

            Assert.AreEqual(expectedAge, customer.Age);
        }

        [Test]
        public void ToString_ShouldReturnFormattedString()
        {
            var customer = new Customer("Alice", "Smith", "Female", new DateTime(1995, 3, 15));
            string text = customer.ToString();

            StringAssert.Contains("Alice Smith", text);
            StringAssert.Contains("Female", text);
        }

        [Test]
        public void SaveAndLoad_ShouldPersistCustomers()
        {
            new Customer("Alice", "Smith", "Female", new DateTime(1995, 3, 15));
            new Customer("Bob", "Jones", "Male", new DateTime(1988, 7, 20));

            Customer.Save(filePath);
            Assert.That(File.Exists(filePath), "File should be created");

            Customer.ClearAllCustomers();

            Customer.Load(filePath);

            Assert.That(Customer.Customers.Count, Is.EqualTo(2), "Should load 2 customers");
            Assert.That(Customer.Customers[0].Name, Is.EqualTo("Alice"), "First customer name should be 'Alice'");
            Assert.That(Customer.Customers[1].Name, Is.EqualTo("Bob"), "Second customer name should be 'Bob'");
        }
    }
}