namespace CinemaManagementSystem.ContractTypeForEmployee
{
    [Serializable]
    public class PartTimeContract : ContractType
    {
        private int _hoursPerWeek;

        public int HoursPerWeek
        {
            get => _hoursPerWeek;
            set
            {
                if (value <= 0 || value > MaxHours * 5)
                    throw new ArgumentException("Part-time employee must work between 1 and 30 hours.");
                _hoursPerWeek = value;
            }
        }
        
        private static int MaxHours = 6;

        public PartTimeContract() { }

        public PartTimeContract(int hoursPerWeek)
        {
            HoursPerWeek = hoursPerWeek;
        }

        public override string ToString()
        {
            return base.ToString() + 
                   $", Part-Time ({HoursPerWeek}h/week)";
        }

        public override string Name => "PartTime";
    }
}