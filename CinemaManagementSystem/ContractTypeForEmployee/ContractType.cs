namespace CinemaManagementSystem.ContractTypeForEmployee
{
    [Serializable]
    public abstract class ContractType
    {
        public abstract string Name { get; }

        protected ContractType() { }
    }
}