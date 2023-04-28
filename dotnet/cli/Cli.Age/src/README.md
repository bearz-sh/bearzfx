# Bearz.Extensions.CliCommand.Age
<a name="top"></a>

## Description

Empowers C# to invoke the [age][age] encryption tool from the command line.

<p align="right">(<a href="#top">back to top</a>)</p>

## Features 

```csharp
var age = new AgeCommand()
 .WithArgs(
    new AgeArgsBuilder()
    .WithInput("input.txt")
    .WithEncrypt(true)
    .WithOutput("output.txt")
  )
  .WithStudio(Stdio.Inherit);
    
var result = age.Output()
   .ThrowOnInvalidExitCode();

Console.WriteLine(result.ExitCode);
```

<p align="right">(<a href="#top">back to top</a>)</p>

## Installation

```powershell
dotnet add package Bearz.Extensions.CliCommand.Age
```

```powershell 
<PackageReference Include="Bearz.Extensions.CliCommand.Age" Version="*" />
```

<p align="right">(<a href="#top">back to top</a>)</p>

## License 

MIT License

Copyright (c) 2022 bearz-sh

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.


<p align="right">(<a href="#top">back to top</a>)</p>

[age]:https://github.com/FiloSottile/age
