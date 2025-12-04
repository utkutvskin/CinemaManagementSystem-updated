// Persistence.cs

using System.Xml.Serialization;

namespace CinemaManagementSystem.PersistenceForAllClasses
{
    public static class Persistence
    {
        public static void SaveAll(string filePath)
        {
            var extent = Extent.Capture();
            var serializer = new XmlSerializer(typeof(Extent));

            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            serializer.Serialize(fs, extent);
        }
        
        
        public static bool LoadAll(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            var serializer = new XmlSerializer(typeof(Extent));

            Extent? loaded;
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    loaded = serializer.Deserialize(fs) as Extent;
                }
                catch
                {
                    return false;
                }
            }

            if (loaded == null)
                return false;

            loaded.Apply();
            return true;
        }
    }
}