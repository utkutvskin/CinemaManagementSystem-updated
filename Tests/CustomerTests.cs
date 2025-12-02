using System;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class CustomerTests
    {
        [SetUp]
        public void SetUp()
        {
            Customer.ClearAllCustomers();
        }

        [TearDown]
        public void TearDown()
        {
            Customer.ClearAllCustomers();
        }

        [Test]
        public void Name_SetEmpty_ThrowsArgumentException()
        {
            var customer = new Customer();
            Assert.Throws<ArgumentException>(() => customer.Name = "");
            Assert.Throws<ArgumentException>(() => customer.Name = "   ");
        }

        [Test]
        public void Surname_SetEmpty_ThrowsArgumentException()
        {
            var customer = new Customer();
            Assert.Throws<ArgumentException>(() => customer.Surname = "");
            Assert.Throws<ArgumentException>(() => customer.Surname = "   ");
        }

        [Test]
        public void Gender_SetEmpty_ThrowsArgumentException()
        {
            var customer = new Customer();
            Assert.Throws<ArgumentException>(() => customer.Gender = "");
            Assert.Throws<ArgumentException>(() => customer.Gender = "   ");
        }

        [Test]
        public void BirthDate_InFuture_ThrowsArgumentException()
        {
            var customer = new Customer();
            Assert.Throws<ArgumentException>(() => customer.BirthDate = DateTime.Now.AddDays(1));
        }

        [Test]
        public void Age_Computed_From_BirthDate_Correctly()
        {
            // Use a fixed birth date to keep test stable
            var birth = new DateTime(1992, 4, 10);
            var customer = new Customer("John", "Doe", "M", birth);

            var today = DateTime.Today;
            int expectedAge = today.Year - birth.Year;
            if (birth > today.AddYears(-expectedAge)) expectedAge--;

            Assert.AreEqual(expectedAge, customer.Age);
        }

        [Test]
        public void SaveAndLoad_PreservesFields_And_Age_Computed_After_Load()
        {
            var birth = new DateTime(1985, 11, 5);
            var customer = new Customer("Alice", "Brown", "F", birth);

            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".xml");
            try
            {
                Customer.Save(tempFile);

                // Clear memory and load from file
                Customer.ClearAllCustomers();
                Assert.IsEmpty(Customer.Customers);

                Customer.Load(tempFile);

                Assert.AreEqual(1, Customer.Customers.Count);
                var loaded = Customer.Customers[0];

                Assert.AreEqual("Alice", loaded.Name);
                Assert.AreEqual("Brown", loaded.Surname);
                Assert.AreEqual("F", loaded.Gender);
                Assert.AreEqual(birth, loaded.BirthDate);

                // Age must be computed from BirthDate (not read from file)
                var today = DateTime.Today;
                int expectedAge = today.Year - birth.Year;
                if (birth > today.AddYears(-expectedAge)) expectedAge--;
                Assert.AreEqual(expectedAge, loaded.Age);
            }
            finally
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
            }
        }
    }
}
