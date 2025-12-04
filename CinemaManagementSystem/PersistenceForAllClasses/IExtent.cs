namespace CinemaManagementSystem.PersistenceForAllClasses;


public interface IExtent<T>
{
    List<T> GetExtent();

    void ReplaceExtent(List<T> newExtent);
}