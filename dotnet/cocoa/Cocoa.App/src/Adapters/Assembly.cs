// Copyright © 2017 - 2022 Chocolatey Software, Inc
// Copyright © 2011 - 2017 RealDimensions Software, LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
//
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure/adapters/Assembly.cs

using System.ComponentModel;
using System.Reflection;

namespace Cocoa.Adapters;

public sealed class Assembly : IAssembly
{
    private readonly System.Reflection.Assembly assembly;

    private Assembly(System.Reflection.Assembly assembly)
    {
        this.assembly = assembly;
    }

    public string? FullName => this.assembly.FullName;

    public string Location => this.assembly.Location;

#pragma warning disable SYSLIB0012 // may be needed for full framework
    public string? CodeBase => this.assembly.CodeBase;
#pragma warning restore SYSLIB0012

    [EditorBrowsable(EditorBrowsableState.Never)]
    public System.Reflection.Assembly UnderlyingType => this.assembly;

    public static implicit operator Assembly(System.Reflection.Assembly value)
    {
        return new Assembly(value);
    }

    public static IAssembly Load(byte[] rawAssembly)
    {
        return new Assembly(System.Reflection.Assembly.Load(rawAssembly));
    }

    public static IAssembly Load(byte[] rawAssembly, byte[] rawSymbols)
    {
        return new Assembly(System.Reflection.Assembly.Load(rawAssembly, rawSymbols));
    }

    public static IAssembly LoadFile(string path)
    {
        return new Assembly(System.Reflection.Assembly.Load(path));
    }

    public static IAssembly? GetAssembly(Type type)
    {
        var assembly = System.Reflection.Assembly.GetAssembly(type);
        if (assembly is null)
            return null;

        return new Assembly(assembly);
    }

    public static IAssembly GetExecutingAssembly()
    {
        return new Assembly(System.Reflection.Assembly.GetExecutingAssembly());
    }

    public static IAssembly GetCallingAssembly()
    {
        return new Assembly(System.Reflection.Assembly.GetCallingAssembly());
    }

    public static IAssembly? GetEntryAssembly()
    {
        var assembly = System.Reflection.Assembly.GetEntryAssembly();
        if (assembly is null)
            return null;

        return new Assembly(assembly);
    }

    public static IAssembly SetAssembly(System.Reflection.Assembly value)
    {
        return new Assembly(value);
    }

    public string[] GetManifestResourceNames()
        => this.assembly.GetManifestResourceNames();

    public Stream? GetManifestResourceStream(string name)
        => this.assembly.GetManifestResourceStream(name);

    public Stream? GetManifestResourceStream(Type type, string name)
        => this.assembly.GetManifestResourceStream(type, name);

    public AssemblyName GetName()
        => this.assembly.GetName();

    public Type? GetType(string name)
        => this.assembly.GetType(name);

    public Type? GetType(string name, bool throwOnError)
        => this.assembly.GetType(name, throwOnError);

    public Type? GetType(string name, bool throwOnError, bool ignoreCase)
        => this.assembly.GetType(name, throwOnError);

    public Type[] GetTypes()
        => this.assembly.GetTypes();
}