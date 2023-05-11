namespace Plank.Tasks;

public interface IMutableVariables : IVariables
{
    new object? this[string name] { get; set; }
}