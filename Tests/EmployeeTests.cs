using System;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class EmployeeTests
    {
        private string _filePath;

        [SetUp]
        public void SetUp()
        {
            // use a temp file per test to avoid collisions
            _filePath = Path.Combine(Path.GetTempPath(), "employees_test_" + Guid.NewGuid() + ".xml");

            // ensure clean static state
            Employee.ClearAllEmployees();

            if (File.Exists(_filePath))
                File.Delete(_filePath);
        }

        [TearDown]
        public void TearDown()
        {
            Employee.ClearAllEmployees();

            try
            {
                if (File.Exists(_filePath))
                    File.Delete(_filePath);
            }
            catch { /* ignore cleanup errors */ }
        }

        [Test]
        public void Name_SetEmpty_ThrowsArgumentException()
        {
            var emp = new Employee();
            Assert.Throws<ArgumentException>(() => emp.Name = "");
            Assert.Throws<ArgumentException>(() => emp.Name = "   ");
        }

        [Test]
        public void Surname_SetEmpty_ThrowsArgumentException()
        {
            var emp = new Employee();
            Assert.Throws<ArgumentException>(() => emp.Surname = "");
            Assert.Throws<ArgumentException>(() => emp.Surname = "   ");
        }

        [Test]
        public void BirthDate_InFuture_ThrowsArgumentException()
        {
            // constructor uses setters, so providing future birth date should throw
            Assert.Throws<ArgumentException>(() =>
                new Employee("Jane", "Doe", DateTime.Now.AddYears(1), DateTime.Now, 1000));
        }

        [Test]
        public void StartDate_InFuture_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Employee("John", "Smith", new DateTime(1990, 1, 1), DateTime.Now.AddDays(1), 1500));
        }

        [Test]
        public void EndDate_BeforeStart_ThrowsArgumentException()
        {
            // create a valid employee, then set EndDate earlier than StartDate
            var emp = new Employee("A", "B", new DateTime(1990, 1, 1), new DateTime(2020, 1, 1), 1000);
            Assert.Throws<ArgumentException>(() => emp.EndDate = new DateTime(2019, 12, 31));
        }

        [Test]
        public void Salary_NonPositive_ThrowsArgumentException()
        {
            // 0 and negative salary are invalid via constructor validation
            Assert.Throws<ArgumentException>(() =>
                new Employee("Sam", "Smith", new DateTime(1995, 5, 5), new DateTime(2020, 1, 1), 0));
            Assert.Throws<ArgumentException>(() =>
                new Employee("Sam", "Smith", new DateTime(1995, 5, 5), new DateTime(2020, 1, 1), -100));
        }

        // Extra tests to ensure construction and persistence still work
        [Test]
        public void Constructor_ValidData_ShouldCreateEmployee()
        {
            var emp = new Employee("John", "Doe", new DateTime(1990, 1, 1), new DateTime(2015, 6, 1), 2500);

            Assert.AreEqual("John", emp.Name);
            Assert.AreEqual("Doe", emp.Surname);
            Assert.AreEqual(2500, emp.Salary);

            // constructor adds to static list
            Assert.IsNotEmpty(Employee.Employees);
            Assert.AreEqual(emp, Employee.Employees[0]);
        }

        [Test]
        public void SaveAndLoad_ShouldPersistEmployees()
        {
            var emp = new Employee("John", "Doe", new DateTime(1990, 1, 1), new DateTime(2015, 6, 1), 2500);

            // Save current employees to file
            Employee.Save(_filePath);

            // Clear in-memory list to ensure Load restores data
            Employee.ClearAllEmployees();
            Assert.IsEmpty(Employee.Employees);

            // Load from file
            Employee.Load(_filePath);

            Assert.IsNotEmpty(Employee.Employees);
            var loaded = Employee.Employees[0];

            Assert.AreEqual("John", loaded.Name);
            Assert.AreEqual("Doe", loaded.Surname);
            Assert.AreEqual(2500, loaded.Salary);

            // verify computed Age is consistent
            var today = DateTime.Today;
            var birth = new DateTime(1990, 1, 1);
            int expectedAge = today.Year - birth.Year;
            if (birth > today.AddYears(-expectedAge)) expectedAge--;
            Assert.AreEqual(expectedAge, loaded.Age);
        }
    }
}
