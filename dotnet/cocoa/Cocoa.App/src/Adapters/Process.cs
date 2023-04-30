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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure/adapters/Process.cs

using System.Diagnostics;

namespace Cocoa.Adapters;

public sealed class Process : IProcess
{
    private readonly System.Diagnostics.Process process;

    public Process()
    {
        this.process = new System.Diagnostics.Process();
        this.process.ErrorDataReceived += (sender, args) => this.ErrorDataReceived?.Invoke(sender, args);
        this.process.OutputDataReceived += (sender, args) => this.OutputDataReceived?.Invoke(sender, args);
    }

    public event EventHandler<DataReceivedEventArgs>? OutputDataReceived;

    public event EventHandler<DataReceivedEventArgs>? ErrorDataReceived;

    public ProcessStartInfo StartInfo
    {
        get => this.process.StartInfo;
        set => this.process.StartInfo = value;
    }

    public bool EnableRaisingEvents
    {
        get => this.process.EnableRaisingEvents;
        set => this.process.EnableRaisingEvents = value;
    }

    public int ExitCode => this.process.ExitCode;

    public System.Diagnostics.Process UnderlyingType => this.process;

    public void Start()
        => this.process.Start();

    public void BeginErrorReadLine()
        => this.process.BeginErrorReadLine();

    public void BeginOutputReadLine()
        => this.process.BeginOutputReadLine();

    public void WaitForExit()
        => this.process.WaitForExit();

    public bool WaitForExit(int milliseconds)
        => this.process.WaitForExit(milliseconds);

    public void Dispose()
        => this.process.Dispose();
}