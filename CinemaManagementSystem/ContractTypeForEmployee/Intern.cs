namespace CinemaManagementSystem.ContractTypeForEmployee
{
    [Serializable]
    public class InternContract : ContractType
    {
        private string _universityName;
        private int _duration;

        public string UniversityName
        {
            get => _universityName;
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("University name cannot be empty");
                _universityName = value;
            }
        }

        public int Duration
        {
            get => _duration;
            set
            {
                if(value < 0)
                    throw new ArgumentException("Duration cannot be negative");
                _duration = value;
            }
        }

        public override string Name => "Intern";

        public InternContract(string universityName, int duration)
        {
            _universityName = universityName;
            _duration = duration;
        }

        public override string ToString()
        {
            return base.ToString() + 
                   $", Intern (University {UniversityName}, Duration: {Duration})";
        }
    }
}