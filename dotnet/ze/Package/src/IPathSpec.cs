namespace Ze.Package.Actions;

public interface IPathSpec
{
    string ZeRootDir { get; }

    string EtcDir { get; }

    string BinDir { get; }

    string DataDir { get; }

    string RunDir { get; }

    string LogDir { get; }

    string ComposeDir { get; }
}