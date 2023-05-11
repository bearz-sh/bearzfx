using System.Collections.ObjectModel;

namespace Ze.Tasks;

public sealed class JobCollection : KeyedCollection<string, IJob>
{
    public JobCollection()
        : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    internal void CheckForMissingDependencies()
    {
        var missingDependencies = new SortedDictionary<string, SortedSet<string>>();

        foreach (var target in this)
        {
            foreach (var dependency in target.Dependencies.Where(dependency => !this.Contains(dependency.Id)))
            {
                if (!missingDependencies.TryGetValue(dependency.Id, out var set))
                {
                    set = new SortedSet<string>();
                    missingDependencies[dependency.Id] = set;
                }

                set.Add(target.Id);
            }
        }

        if (missingDependencies.Count == 0)
        {
            return;
        }

        var message =
            $"Missing {(missingDependencies.Count > 1 ? "dependencies" : "dependency")}: " +
            string.Join(
                "; ",
                missingDependencies.Select(dependency =>
                    $"{dependency.Key}, required by {dependency.Value}"));

        throw new InvalidOperationException(message);
    }

    internal void CheckForCircularDependencies()
    {
        var dependents = new Stack<string>();

        foreach (var job in this)
        {
            Check(job);
        }

        void Check(IJob target)
        {
            if (dependents.Contains(target.Name))
            {
                throw new InvalidOperationException($"Circular dependency: {string.Join(" -> ", dependents.Reverse().Append(target.Name))}");
            }

            dependents.Push(target.Name);

            foreach (var dependency in target.Dependencies.Where(o => this.Contains(o.Id)))
            {
                Check(this[dependency.Id]);
            }

            _ = dependents.Pop();
        }
    }

    protected override string GetKeyForItem(IJob item)
    {
        return item.Id;
    }
}