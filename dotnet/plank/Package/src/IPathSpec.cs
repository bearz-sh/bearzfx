namespace Plank.Package.Actions;

public interface IPathSpec
{
    string PlankRootDir { get; }

    string EtcDir { get; }

    string BinDir { get; }

    string DataDir { get; }

    string RunDir { get; }

    string LogDir { get; }
    
    string ComposeDir { get; }
}