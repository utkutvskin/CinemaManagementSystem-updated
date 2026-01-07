using System;

namespace CinemaManagementSystem.Items
{
    [Serializable]
    public class Glass3D : Item
    {
        private string _size;

        public string Size
        {
            get => _size;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Size cannot be null or empty");
                _size = value;
            }
        }
        
        //class extent
        private static List<Glass3D> _glass3Ds = new List<Glass3D>();
        public static IReadOnlyList<Glass3D> Glass3Ds => _glass3Ds.AsReadOnly();
        
        private static void AddGlass(Glass3D glass)
        {
            if (glass == null)
                throw new ArgumentException("glass cannot be null");

            _glass3Ds.Add(glass);
        }

        public Glass3D(string size, double price, int availableQuantity)
            : base($"3D Glasses with size {size}", price, availableQuantity)
        {
            Size = size;
            
            AddGlass(this);
        }
    }
}
