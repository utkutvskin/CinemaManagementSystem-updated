namespace CinemaManagementSystem.ContractTypeForEmployee
{
    [Serializable]
    public class FullTimeContract : ContractType
    {
        private readonly Dictionary<DateTime, double> _bonuses = new();

        public override string Name => "FullTime";

        public IReadOnlyDictionary<DateTime, double> Bonuses => _bonuses;

        public FullTimeContract() { }

        public void AddBonus(double bonus)
        {
            _bonuses.Add(DateTime.Now, bonus);
        }

        public override string ToString()
        {
            var str = "";
            foreach (var bonus in _bonuses)
            {
                str += $"Got {bonus.Value} bonus at {bonus.Key}\n";
            }
            return str;
        }
    }
}