using System. Xml.Serialization;
using CinemaManagementSystem.Area;
using CinemaManagementSystem.AssociationClasses;
using CinemaManagementSystem. Enums;
using CinemaManagementSystem.Person;

namespace CinemaManagementSystem.Employees
{
    [Serializable]
    public class Cleaner : Employee
    {
        private CleaningTypeEnum _cleaningType;

        public CleaningTypeEnum CleaningType
        {
            get => _cleaningType;
            set => _cleaningType = value;
        }


        // attribute association:  CleanerAssignment 

        [XmlIgnore]
        private List<CleanerAssignment> _assignments = new();

        [XmlIgnore]
        public IReadOnlyCollection<CleanerAssignment> Assignments 
            => _assignments.AsReadOnly();

        internal void AddCleanerAssignmentInternal(CleanerAssignment assignment)
        {
            if (assignment == null)
                throw new ArgumentException("Assignment cannot be null.");

            _assignments.Add(assignment);
        }

        internal void RemoveCleanerAssignmentInternal(CleanerAssignment assignment)
        {
            if(assignment == null) throw new ArgumentException("Assignment cannot be null.");
            _assignments.Remove(assignment);
        }

        public CleanerAssignment Clean(CleanableArea area)
        {
            if(area == null)
                throw new ArgumentException("Area cannot be null.");
            
            var now = DateTime.Now;

            return CleanerAssignment.Create(this, area, now. Date, now.TimeOfDay);
        }


        // Constructors

        public Cleaner() { }

        public Cleaner(string name, string surname, DateTime birthDate, GenderEnum gender, CleaningTypeEnum cleaningType)
            : base(name, surname, birthDate, gender, Role.Cleaner)
        {
            CleaningType = cleaningType;
        }


        // Methods
        public List<CleanableArea> GetListOfAreaThatNeedToBeCleaned()
        {
            return CleanableArea
                .GenerateListOfAreaToClean()
                .ToList();
        }


        public override string ToString()
        {
            return $"Cleaner ({CleaningType}), Assignments: {_assignments.Count}";
        }
    }
}