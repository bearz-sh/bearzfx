// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.Json;

var first = args.Length > 0 ? args[0].ToLower().Trim() : "default";
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
switch (first)
{
    case "default":
        Console.WriteLine("Hello, World!");
        Environment.Exit(0);
        break;
    case "error":
        Console.Error.WriteLine("Hello, World!");
        Environment.Exit(1);
        break;
    case "streams":
        Console.WriteLine("Hello, World!");
        Console.Error.WriteLine("Error line");
        Environment.Exit(0);
        break;
    case "cwd":
        Console.WriteLine(Environment.CurrentDirectory);
        Environment.Exit(0);
        break;
    case "dump-env":
        var d = Environment.GetEnvironmentVariables();
        var set = new Dictionary<string, string>();
        foreach (var key in d.Keys)
        {
            if (key is string s)
            {
                set[s] = Environment.GetEnvironmentVariable(s) ?? "null";
            }
        }

        var json = JsonSerializer.Serialize(set, new JsonSerializerOptions
        {
            WriteIndented = true,
        });

        Console.WriteLine(json);

        break;
    case "prompt":
    case "echo":
        Console.WriteLine("echo mode");
        Console.WriteLine("Enter a value: ");
        while (Console.ReadLine() is { } line)
        {
            if (line.Trim() == "exit")
            {
                Console.WriteLine("exiting...");
                Environment.Exit(0);
                break;
            }

            Console.WriteLine($"You entered: {line}");
            Console.WriteLine("Enter a value: ");
        }

        break;
    default:
        Console.WriteLine(string.Join(" ", args));
        break;
}

Environment.Exit(0);