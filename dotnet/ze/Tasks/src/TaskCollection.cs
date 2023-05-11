using System.Collections.ObjectModel;

using Bearz.Extra.Strings;

namespace Ze.Tasks;

public class TaskCollection : Collection<ITask>, IDependencyCollection<ITask>
{
    private readonly Dictionary<string, ITask> lookup = new(StringComparer.OrdinalIgnoreCase);

    public ITask? this[string name]
    {
        get
        {
            if (this.lookup.TryGetValue(name, out var task))
                return task;

            return null;
        }
    }

    public new void Add(ITask item)
    {
        if (!this.lookup.ContainsKey(item.Id))
        {
            this.lookup.Add(item.Id, item);

            base.Add(item);
        }
    }

    public bool Contains(string name)
    {
        return this.lookup.ContainsKey(IdGenerator.Instance.FromName(name.AsSpan()));
    }

    public bool ContainsId(string id)
        => this.lookup.ContainsKey(id);

    public void CheckCircularDependencies()
    {
        var missingDependencies = new SortedDictionary<string, SortedSet<string>>();

        foreach (var target in this)
        {
            foreach (var dependency in target.Dependencies)
            {
                var dep = this[dependency];
                if (dep is not null)
                    continue;

                if (!missingDependencies.TryGetValue(dependency, out var set))
                {
                    set = new SortedSet<string>();
                    missingDependencies[dependency] = set;
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

    public void CheckMissingDependencies()
    {
        // more or less from bullseye
        var dependents = new Stack<string>();

        foreach (var task in this)
        {
            Check(task);
        }

        void Check(ITask task)
        {
            if (dependents.Contains(task.Id))
            {
                throw new InvalidOperationException($"Circular dependency: {string.Join(" -> ", dependents.Reverse().Append(task.Name))}");
            }

            dependents.Push(task.Id);

            foreach (var dependency in task.Dependencies)
            {
                var dep = this[dependency];
                if (dep is null)
                    continue;

                Check(dep);
            }

            _ = dependents.Pop();
        }
    }
}