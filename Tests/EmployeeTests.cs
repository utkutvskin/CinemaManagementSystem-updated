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
            Employee.ClearAllEmployees();
        }

        //Test Name/Surname setter 
        [Test]
        public void NameSetterValidation_ShouldThrowException()
        {
            Employee employee = new Employee("John", "Doe", new DateTime(1990, 1, 1), new DateTime(2015, 6, 1), 3000);
            Assert.Throws<ArgumentException>(() =>
                employee.Name = " "
            );
        }

        [Test]
        public void SurnameSetterValidation_ShouldSetSurnameSuccessfully()
        {
            Employee employee = new Employee("John", "Doe", new DateTime(1990, 1, 1), new DateTime(2015, 6, 1), 3000);
            employee.Surname = "Smith";
            Assert.That(employee.Surname, Is.EqualTo("Smith"));
        }

        //Test BirthDay setter
        [Test]
        public void BirthDaySetterValidation_ShouldThrowException()
        {
            Employee employee = new Employee("John", "Doe", new DateTime(1990, 1, 1), new DateTime(2015, 6, 1), 3000);
            Assert.Throws<ArgumentException>(() =>
                employee.BirthDate = new DateTime(2024, 3, 4)
            );
        }

        [Test]
        public void BirthDaySetterValidation_ShouldSetBirthDaySuccessfully()
        {
            Employee employee = new Employee("John", "Doe", new DateTime(1990, 1, 1), new DateTime(2015, 6, 1), 3000);
            employee.BirthDate = new DateTime(2005, 3, 4);
            Assert.That(employee.BirthDate, Is.EqualTo(new DateTime(2005, 3, 4)));
        }

        //Test start date
        [Test]
        public void StartDateSetterValidation_ShouldThrowException()
        {
            Employee employee = new Employee("John", "Doe", new DateTime(1990, 1, 1), new DateTime(2015, 6, 1), 3000);
            Assert.Throws<ArgumentException>(() =>
                employee.StartDate = new DateTime(2030, 3, 4)
            );
        }

        [Test]
        public void StartDateSetterValidation_ShouldSetStartDateSuccessfully()
        {
            Employee employee = new Employee("John", "Doe", new DateTime(1990, 1, 1), new DateTime(2015, 6, 1), 3000);
            employee.StartDate = new DateTime(2010, 3, 4);
            Assert.That(employee.StartDate, Is.EqualTo(new DateTime(2010, 3, 4)));
        }

        //Test end date
        [Test]
        public void EndDateSetterValidation_ShouldThrowException()
        {
            Employee employee = new Employee("John", "Doe", new DateTime(1990, 1, 1), new DateTime(2015, 6, 1), 3000);
            Assert.Throws<ArgumentException>(() =>
                employee.EndDate = new DateTime(1986, 3, 4)
            );
        }

        [Test]
        public void EndDateSetterValidation_ShouldSetEndDateSuccessfully()
        {
            Employee employee = new Employee("John", "Doe", new DateTime(1990, 1, 1), new DateTime(2015, 6, 1), 3000);
            employee.EndDate = new DateTime(2020, 3, 4);
            Assert.That(employee.EndDate, Is.EqualTo(new DateTime(2020, 3, 4)));
        }

        //test salary
        [Test]
        public void SalaryValidation_ShouldThrowException()
        {
            Employee employee = new Employee("John", "Doe", new DateTime(1990, 1, 1), new DateTime(2015, 6, 1), 3000);
            Assert.Throws<ArgumentException>(() =>
                employee.Salary = 2500
            );
        }

        [Test]
        public void SalaryValidation_ShouldSetSuccessfully()
        {
            Employee employee = new Employee("John", "Doe", new DateTime(1990, 1, 1), new DateTime(2015, 6, 1), 3000);
            employee.Salary = 4000;
            Assert.That(employee.Salary, Is.EqualTo(4000));
        }

        [Test]
        public void Constructor_ValidData_ShouldCreateEmployee()
        {
            var emp = new Employee("John", "Doe", new DateTime(1990, 1, 1), new DateTime(2015, 6, 1), 3000);

            Assert.AreEqual("John", emp.Name);
            Assert.AreEqual("Doe", emp.Surname);
            Assert.AreEqual(3000, emp.Salary);
        }

        [Test]
        public void Constructor_FutureBirthDate_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Employee("Jane", "Doe", DateTime.Now.AddYears(1), DateTime.Now, 3000));
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
            if (File.Exists(FilePath))
                File.Delete(FilePath);

            var emp = new Employee("John", "Doe", new DateTime(1990, 1, 1), new DateTime(2015, 6, 1), 3000);

            Employee.Save(FilePath);
            Assert.That(File.Exists(FilePath), "File should be created");

            Employee.ClearAllEmployees();

            Employee.Load(FilePath);

            Assert.That(Employee.Employees.Count, Is.EqualTo(1), "Should load 1 employee");
            Assert.That(Employee.Employees[0].Name, Is.EqualTo("John"), "Name should be 'John'");
            Assert.That(Employee.Employees[0].Surname, Is.EqualTo("Doe"), "Surname should be 'Doe'");
            Assert.That(Employee.Employees[0].Salary, Is.EqualTo(3000), "Salary should be 3000");
        }

        
    }
}
