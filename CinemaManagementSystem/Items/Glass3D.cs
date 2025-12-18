using System;

namespace CinemaManagementSystem.Items
{
    [Serializable]
    public class Glass3D : Item
    {
        // Reusable 3D glasses item
        public bool IsReusable { get; }

        public Glass3D(double price, bool isReusable)
            : base("3D Glasses", price)
        {
            IsReusable = isReusable;
        }
    }
}
