using System;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;
using CinemaManagementSystem.Person;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class CustomerTests
    {
        private string filePath = "customers_test.xml";

        [SetUp]
        public void SetUp()
        {

            Customer.ClearAllCustomers();
        }

        //Test Name/Surname setter 
        [Test]
        public void NameSetterValidation_ShouldThrowException()
        {
            Customer customer = new Customer("John", "Doe", new DateTime(1990, 5, 10));
            Assert.Throws<ArgumentException>(() =>
                customer.Name = " "
            );
        }
        [Test]
        public void SurnameSetterValidation_ShouldSetSurnameSuccessfully()
        {
            Customer customer = new Customer("John", "Doe", new DateTime(1990, 5, 10));
            customer.Surname = "Smith";
            Assert.That(customer.Surname, Is.EqualTo("Smith"));
        }
        
        //Test BirthDay setter
        [Test]
        public void BirthDaySetterValidation_ShouldThrowException()
        {
            Customer customer = new Customer("John", "Doe", new DateTime(1990, 5, 10));
            Assert.Throws<ArgumentException>(() =>
                customer.DateOfBirth = new DateTime(2024, 3,4)
            );

        }

        [Test]
        public void BirthDateSetterValidation_ShouldSetBirthDateSuccessfully()
        {
            Customer customer = new Customer("John", "Doe", new DateTime(1990, 5, 10));
            customer.DateOfBirth = new DateTime(2005, 3, 4);
            Assert.That(customer.DateOfBirth, Is.EqualTo(new DateTime(2005, 3, 4)));
        }
        
        
        [Test]
        public void Constructor_ValidData_ShouldCreateCustomer()
        {
            var customer = new Customer("Alice", "Smith", new DateTime(1995, 3, 15));

            Assert.AreEqual("Alice", customer.Name);
            Assert.AreEqual("Smith", customer.Surname);
            Assert.AreEqual(1995, customer.DateOfBirth.Year);
        }

        [Test]
        public void Constructor_EmptyName_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Customer("", "Smith", new DateTime(1995, 3, 15))
            );
        }

        [Test]
        public void Constructor_EmptySurname_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Customer("Alice", "", new DateTime(1995, 3, 15))
            );
        }

        [Test]
        public void Constructor_FutureBirthDate_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Customer("Alice", "Smith", DateTime.Now.AddYears(1))
            );
        }

        [Test]
        public void Age_ShouldCalculateCorrectly()
        {
            var birthDate = new DateTime(2000, 6, 15);
            var customer = new Customer("Bob", "Jones", birthDate);

            int expectedAge = DateTime.Now.Year - 2000;
            if (DateTime.Now.DayOfYear < birthDate.DayOfYear)
                expectedAge--;

            Assert.AreEqual(expectedAge, customer.Age);
        }

        [Test]
        public void ToString_ShouldReturnFormattedString()
        {
            var customer = new Customer("Alice", "Smith", new DateTime(1995, 3, 15));
            string text = customer.ToString();

            StringAssert.Contains("Alice Smith", text);
        }

     [Test]
public void AddStampcard_ShouldCreateReverseConnection()
{
    var customer = new Customer("John", "Walker", new DateTime(1990, 1, 1));
    var card = new Stampcard();

    customer.AddStampcard(card);

    Assert.AreEqual(customer, card.Customer);
    Assert.IsTrue(customer.Stampcards.ContainsKey(card.DateOfPurchase.Date));
}

[Test]
public void AddStampcard_ShouldThrow_WhenCustomerAlreadyHasOne()
{
    var customer = new Customer("Emma", "Smith", new DateTime(1993, 2, 2));
    var c1 = new Stampcard();
    var c2 = new Stampcard();

    customer.AddStampcard(c1);

    Assert.Throws<InvalidOperationException>(() => customer.AddStampcard(c2));
}

[Test]
public void AddStampcard_ShouldThrow_WhenDateOfPurchaseDuplicate()
{
    var customer = new Customer("David", "Brown", new DateTime(1988, 3, 3));
    var date = new DateTime(2024, 1, 1);

    var card1 = new Stampcard();
    card1.DateOfPurchase = date;

    var card2 = new Stampcard();
    card2.DateOfPurchase = date;

    customer.AddStampcard(card1);

    Assert.Throws<InvalidOperationException>(() => customer.AddStampcard(card2));
}

[Test]
public void RemoveStampcard_ShouldRemoveReverseConnection()
{
    var customer = new Customer("Sophia", "Johnson", new DateTime(1991, 4, 4));
    var card = new Stampcard();

    customer.AddStampcard(card);
    customer.RemoveStampcard(card);

    Assert.IsNull(card.Customer);
    Assert.IsFalse(customer.Stampcards.ContainsKey(card.DateOfPurchase.Date));
}

[Test]
public void RemoveStampcard_ShouldThrow_WhenCardNotAssociated()
{
    var customer = new Customer("Michael", "Green", new DateTime(1995, 6, 6));
    var card = new Stampcard();

    Assert.Throws<InvalidOperationException>(() => customer.RemoveStampcard(card));
}

[Test]
public void SetCustomer_ShouldMoveCardToAnotherCustomer()
{
    var c1 = new Customer("Liam", "Taylor", new DateTime(1980, 7, 7));
    var c2 = new Customer("Olivia", "Turner", new DateTime(1985, 8, 8));

    var card = new Stampcard();
    c1.AddStampcard(card);

    card.SetCustomer(c2);

    Assert.AreEqual(c2, card.Customer);
    Assert.IsFalse(c1.Stampcards.ContainsKey(card.DateOfPurchase.Date));
    Assert.IsTrue(c2.Stampcards.ContainsKey(card.DateOfPurchase.Date));
}

        
       


        
        [Test]
        public void SaveAndLoad_ShouldPersistCustomers()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            
            new Customer("Alice", "Smith", new DateTime(1995, 3, 15));
            new Customer("Bob", "Jones", new DateTime(1988, 7, 20));

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
