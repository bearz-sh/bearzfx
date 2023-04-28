using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Reflection;

namespace Bearz.PowerShell.Standard;

public abstract class PsModuleAssemblyLoader : IModuleAssemblyInitializer, IModuleAssemblyCleanup
{
    private static readonly HashSet<string> s_dependencies;
    private static readonly string s_dependencyFolder;
    private static readonly AssemblyLoadContextProxy? s_proxy;

#pragma warning disable S3963
    static PsModuleAssemblyLoader()
#pragma warning restore S3963
    {
        var assembly = typeof(PsModuleAssemblyLoader).Assembly;
        s_dependencyFolder = Path.Combine(Path.GetDirectoryName(assembly.Location));
        s_dependencies = new(StringComparer.Ordinal);
        foreach (string filePath in Directory.EnumerateFiles(s_dependencyFolder, "*.dll"))
        {
            s_dependencies.Add(AssemblyName.GetAssemblyName(filePath).FullName);
        }

        s_proxy = AssemblyLoadContextProxy.Create(assembly.FullName);
    }

    public void OnImport()
    {
        AppDomain.CurrentDomain.AssemblyResolve += ResolvingHandler;
    }

    public void OnRemove(PSModuleInfo psModuleInfo)
    {
        AppDomain.CurrentDomain.AssemblyResolve -= ResolvingHandler;
    }

    internal static Assembly? ResolvingHandler(object sender, ResolveEventArgs args)
    {
        var assemblyName = new AssemblyName(args.Name);
        if (IsAssemblyMatching(assemblyName, args.RequestingAssembly))
        {
            string fileName = assemblyName.Name + ".dll";
            string filePath = Path.Combine(s_dependencyFolder, fileName);

            if (File.Exists(filePath))
            {
                Console.WriteLine($"<*** Fall in 'ResolvingHandler': Newtonsoft.Json, Version=13.0.0.0  -- Loaded! ***>");

                // - In .NET, load the assembly into the custom assembly load context.
                // - In .NET Framework, assembly conflict is not a problem, so we load the assembly
                //   by 'Assembly.LoadFrom', the same as what powershell.exe would do.
                return s_proxy is not null
                    ? s_proxy.LoadFromAssemblyPath(filePath)
#pragma warning disable S3885
                    : Assembly.LoadFrom(filePath);
#pragma warning restore S3885
            }
        }

        return null;
    }

    private static bool IsAssemblyMatching(AssemblyName assemblyName, Assembly? requestingAssembly)
    {
        // The requesting assembly is always available in .NET, but could be null in .NET Framework.
        // - When the requesting assembly is available, we check whether the loading request came from this
        //   module (the 'conflict' assembly in this case), so as to make sure we only act on the request
        //   from this module.
        // - When the requesting assembly is not available, we just have to depend on the assembly name only.
        return requestingAssembly is not null
            ? requestingAssembly.FullName.StartsWith("conflict,") && s_dependencies.Contains(assemblyName.FullName)
            : s_dependencies.Contains(assemblyName.FullName);
    }

    internal class AssemblyLoadContextProxy
    {
        private readonly object customContext;
        private readonly MethodInfo loadFromAssemblyPath;

        private AssemblyLoadContextProxy(Type alc, string loadContextName)
        {
            var ctor = alc.GetConstructor(new[] { typeof(string), typeof(bool) });
            if (ctor is null)
                throw new MissingMethodException(alc.FullName, ".ctor(string, bool)");
            var load = alc.GetMethod("LoadFromAssemblyPath", new[] { typeof(string) });

            this.loadFromAssemblyPath = load ?? throw new MissingMethodException(alc.FullName, "LoadFromAssemblyPath(string)");
            this.customContext = ctor.Invoke(new object[] { loadContextName, false });
        }

        internal static AssemblyLoadContextProxy? Create(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            var alc = typeof(object).Assembly.GetType("System.Runtime.Loader.AssemblyLoadContext");
            return alc is not null
                ? new AssemblyLoadContextProxy(alc, name)
                : null;
        }

        internal Assembly LoadFromAssemblyPath(string assemblyPath)
        {
            return (Assembly)this.loadFromAssemblyPath.Invoke(
                this.customContext,
                new object[] { assemblyPath });
        }
    }
}