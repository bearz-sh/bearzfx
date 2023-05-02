// Copyright © 2017 - 2021 Chocolatey Software, Inc
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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure/adapters/Environment.cs

using System.Runtime.InteropServices;

using Bearz.Std;

namespace Cocoa.Adapters;

public sealed class CocoaEnvironment : IEnvironment
{
    public OperatingSystem OSVersion
    {
        get { return System.Environment.OSVersion; }
    }

    public bool Is64BitOperatingSystem
    {
        get { return System.Environment.Is64BitOperatingSystem; }
    }

    public bool Is64BitProcess
    {
        get
        {
            // ARM64 bit architecture has a x86-32 emulator, so return false
            if (System.Environment.GetEnvironmentVariable(EnvironmentKeys.ProcessorArchitecture).EqualsInvariant(EnvironmentKeys.Arm64ProcessorArchitecture))
            {
                return false;
            }

            return IntPtr.Size == 8;
        }
    }

    public bool UserInteractive => Env.IsUserInteractive;

    public string NewLine => Environment.NewLine;

    public string CurrentDirectory => Environment.CurrentDirectory;

    public string ExpandEnvironmentVariables(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return name;

        return Env.Expand(name);
    }

    public string? GetEnvironmentVariable(string variable)
    {
        return System.Environment.GetEnvironmentVariable(variable);
    }

    public IDictionary<string, string> GetEnvironmentVariables()
        => Env.GetAll();

    public IDictionary<string, string> GetEnvironmentVariables(EnvironmentVariableTarget target)
        => Env.GetAll(target);

    public void SetEnvironmentVariable(string variable, string value)
        => Environment.SetEnvironmentVariable(variable, value);
}