using System;
using System.Linq;

namespace Ze.ClearScript.Core;

public class Class1
{
    public void Test()
    {
        // https://github.com/microsoft/ClearScript/issues/451
        Microsoft.ClearScript.ScriptEngine engine = new Microsoft.ClearScript.V8.V8ScriptEngine();
        Console.WriteLine(engine);
    }
}