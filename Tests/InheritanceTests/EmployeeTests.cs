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
    public void ChangeContractType_ShouldSuccessfullyChange()
    {
        Employee employee = new Employee("Name", "surname", new DateTime(1991,12,3),GenderEnum.Female);
        
        Cleaner cleaner = new Cleaner(CleaningTypeEnum.Deep, employee);
        cleaner.SetFullTime(4000);
        
        cleaner.ChangeToPartTime(28.2);
        
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