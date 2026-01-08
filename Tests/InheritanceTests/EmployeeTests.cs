using CinemaManagementSystem.Enums;
using CinemaManagementSystem.People;
using CinemaManagementSystem.People.Roles;
using NUnit.Framework;

namespace CinemaManagementSystem.Tests.InheritanceTests;

[TestFixture]
public class EmployeeTests
{

    [Test]
    public void CreateEmployeeWithCleanerRoleFullTimeContract_ShouldSuccessfullyCreateEmployee()
    {
        Employee employee = new Employee("Name", "surname", new DateTime(1991,12,3),GenderEnum.Female);
        
        Cleaner cleaner = new Cleaner(CleaningTypeEnum.Deep, employee);
        cleaner.SetFullTime(4000);
        
        Assert.IsInstanceOf<EmployeeRole>(cleaner);
        Assert.That(cleaner.FullTime.Salary, Is.EqualTo(4000));
        Assert.That(cleaner.Employee.Name, Is.EqualTo("Name"));
        
    }
    
    [Test]
    public void CreateEmployeeInternContractUsingConstructor_ShouldSuccessfullyCreateEmployee()
    {
        Employee employee = new Employee("Name", "surname", new DateTime(1991,12,3),GenderEnum.Female);
        
        Cleaner cleaner = new Cleaner(CleaningTypeEnum.Deep, employee, "University name");
        
        Assert.IsInstanceOf<EmployeeRole>(cleaner);
        Assert.That(cleaner.FullTime, Is.EqualTo(null));
        Assert.That(cleaner.PartTime, Is.EqualTo(null));
        Assert.That(cleaner.Intern.UniversityName, Is.EqualTo("University name"));
        
    }
    
    [Test]
    public void SetFullTime_ShouldThrowException_WhenIsAnotherContract()
    {
        Employee employee = new Employee("Name", "surname", new DateTime(1991,12,3),GenderEnum.Female);
        
        Cleaner cleaner = new Cleaner(CleaningTypeEnum.Deep, employee, 3500);
        
        Assert.That(cleaner.Intern, Is.EqualTo(null));
        Assert.That(cleaner.PartTime, Is.EqualTo(null));
        Assert.That(cleaner.FullTime.Salary, Is.EqualTo(3500));
        
        Assert.Throws<InvalidOperationException>(() => cleaner.SetPartTime(34, 40));
        
    }
    [Test]
    public void SetEmployeeRole_ShouldThrowException_WhenIsAnotherRole()
    {
        Employee employee = new Employee("Name", "surname", new DateTime(1991,12,3),GenderEnum.Female);
        
        Cleaner cleaner = new Cleaner(CleaningTypeEnum.Deep, employee, 3500);
        
        Assert.That(employee.CurrentRole, Is.EqualTo(cleaner));
        
        Assert.Throws<InvalidOperationException>(() => new Manager(employee, 5000));
        Assert.That(employee.CurrentRole, Is.EqualTo(cleaner));
        
    }
    [Test]
    public void ChangeEmployeeRole_ShouldThrowException_WhenIsFired()
    {
        Employee employee = new Employee("Name", "surname", new DateTime(1991,12,3),GenderEnum.Female);
        
        Cleaner cleaner = new Cleaner(CleaningTypeEnum.Deep, employee, 3500);
        
        Assert.That(employee.CurrentRole, Is.EqualTo(cleaner));
        employee.IsFired = true;
        
        Assert.Throws<InvalidOperationException>(() => employee.ChangeRoleToReceptionist(2));
        Assert.That(employee.CurrentRole, Is.EqualTo(cleaner));
        
    }
    [Test]
    public void ChangeContractType_ShouldSuccessfullyChange()
    {
        Employee employee = new Employee("Name", "surname", new DateTime(1991,12,3),GenderEnum.Female);
        
        Cleaner cleaner = new Cleaner(CleaningTypeEnum.Deep, employee);
        cleaner.SetFullTime(4000);
        
        cleaner.ChangeToPartTime(28.2, 20);
        
        Assert.That(cleaner.FullTime, Is.EqualTo(null));
        Assert.That(cleaner.PartTime.HourlyRate, Is.EqualTo(28.2));
        
    }
    
    [Test]
    public void ChangeEmployeeRoleFromCleanerToReceptionist_ShouldSuccessfullyChange()
    {
        Employee employee = new Employee("Name", "surname", new DateTime(1991,12,3),GenderEnum.Female);
        
        Cleaner cleaner = new Cleaner(CleaningTypeEnum.Deep, employee);
        cleaner.SetFullTime(4000);
        
        Assert.That(employee.CurrentRole, Is.EqualTo(cleaner));

        var emp = employee.ChangeRoleToReceptionist(2);

        Assert.That(employee.CurrentRole, Is.EqualTo(emp));
        Assert.IsTrue(employee.PrevoiusEmployeeRoles.Contains(cleaner));
        Assert.That(employee.PrevoiusEmployeeRoles.Find(e => e == cleaner).EndDate, Is.Not.Null);
        
    }
}