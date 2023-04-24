using System.Diagnostics;

using Bearz.Diagnostics;
using Bearz.Extra.Collections;
using Bearz.Extra.Strings;

using ChildProcess = System.Diagnostics.Process;

namespace Bearz.Std;

public class Command : CommandBase
{
    public Command(string fileName, CommandStartInfo? startInfo = null)
    {
        this.FileName = fileName;
        this.StartInfo = startInfo ?? new CommandStartInfo();
    }

    public override string FileName { get; }

    public override CommandStartInfo StartInfo { get; }
}