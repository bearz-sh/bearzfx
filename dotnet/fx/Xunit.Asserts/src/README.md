# Bearz.Xunit.Asserts

## Description

A test framework that extends Xunit.net to enable dependency injection,
skipptable attributes, and extensible Asserts with extension methods. The
Bearz.Xunit.Asserts project contains the `IAssert` interface and `FlexAssert`
class. The interface can be used with dependency injection.

FlexAssert has static property called `Default` which can be used to get
the default instance.  Using an instance rather than a static class enables
extension methods to be added to the IAssert interface or FlexAssert class.

The asserts include methods to test `Span&lt;T&gt;` and `Memory&lt;T&gt;`
data and `FlexAssert.Skip` to dynamically skip a test in addition to the
default asserts provided by Xunit.

Examples can be found in the project's test source code.

## Features

- Enables extension methods to be added to the `IAssert` interface.
- Supports all xunit2 asserts.
- Adds asserts from xunit3 around span and memory.
- Adds `Skip()` for skipping tests.
- Enables Bearz.Xunit.Core to inject `IAssert` into test methods.

## Installation

```powershell
dotnet add package Bearz.Standard
```

```powershell
<PackageReference Include="Bearz.Standard" Version="*" />
```

## License

Copyright (c) 2022 - 2023 bearz-sh

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
