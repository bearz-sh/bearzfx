namespace Ze.Tasks;

public interface IDependencyCollection<T> : ICollection<T>
{
    T? this[string name] { get; }

    void CheckCircularDependencies();

    void CheckMissingDependencies();
}