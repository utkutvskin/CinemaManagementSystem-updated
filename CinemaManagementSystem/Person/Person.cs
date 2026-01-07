using System.Xml.Serialization;
using CinemaManagementSystem.Enums;

namespace CinemaManagementSystem.Person;


public abstract class Person
{
    //Attributes
    private string _name;
    private string _surname;
    private GenderEnum _gender;
    private DateTime _birthDate;
    
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty.");
            _name = value;
        }
    }
        
    public string Surname
    {
        get => _surname;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Surname cannot be empty.");
            _surname = value;
        }
    }

    public GenderEnum Gender
    {
        get => _gender;
        set => _gender =value;
    }

    public DateTime BirthDate
    {
        get => _birthDate;
        set
        {
            if(value > DateTime.Now)
                throw new ArgumentException("Birth day cannot be greater than today.");
            _birthDate = value;
        }
    }

    //Derived
    [XmlIgnore]
    public int Age
    {
        get
        {
            int age = DateTime.Now.Year - BirthDate.Year;
            if (DateTime.Now.DayOfYear < BirthDate.DayOfYear)
                age--;
            return age;
        }
    }
    
    //Constructor
    protected Person(){}
    
    protected Person(string name, string surname, GenderEnum gender, DateTime birthDate)
    {
        Name = name;
        Surname = surname;
        Gender = gender;
        BirthDate = birthDate;

    }
    
    //Methods 
    public override string ToString()
    {
        return $"{Name} {Surname}, {Gender}, Age: {Age}";
    }

}