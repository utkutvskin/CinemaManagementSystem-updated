using System;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class EmployeeTests
    {
        private const string FilePath = "employees_test.xml";

        [SetUp]
        public void Setup()
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);

            Employee.ClearAllEmployees();
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(FilePath))
            {
                try { File.Delete(FilePath); }
                catch { /* ignore */ }
            }

            Employee.ClearAllEmployees();
        }

        [Test]
        public void Constructor_ValidData_ShouldCreateEmployee()
        {
            var emp = new Employee("John", "Doe", new DateTime(1990, 1, 1), new DateTime(2015, 6, 1), 2500);
            
            Assert.AreEqual("John", emp.Name);
            Assert.AreEqual("Doe", emp.Surname);
            Assert.AreEqual(2500, emp.Salary);
        }

        [Test]
        public void Constructor_FutureBirthDate_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Employee("Jane", "Doe", DateTime.Now.AddYears(1), DateTime.Now, 1000));
        }

        [Test]
        public void Constructor_NegativeSalary_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Employee("Sam", "Smith", new DateTime(1995, 5, 5), new DateTime(2020, 1, 1), -100));
        }

        [Test]
        public void SaveAndLoad_ShouldPersistEmployees()
        {
            var emp = new Employee("John", "Doe", new DateTime(1990, 1, 1), new DateTime(2015, 6, 1), 2500);
            
            Employee.Save(FilePath);
            Assert.That(File.Exists(FilePath), "File should be created");

            Employee.ClearAllEmployees();

            Employee.Load(FilePath);

            Assert.That(Employee.Employees.Count, Is.EqualTo(1), "Should load 1 employee");
            Assert.That(Employee.Employees[0].Name, Is.EqualTo("John"), "Name should be 'John'");
            Assert.That(Employee.Employees[0].Surname, Is.EqualTo("Doe"), "Surname should be 'Doe'");
            Assert.That(Employee.Employees[0].Salary, Is.EqualTo(2500), "Salary should be 2500");
        }
    }
}