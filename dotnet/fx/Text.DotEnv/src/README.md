# Bearz.Text.DotEnv
<a name="top"></a>

## Description

A .NET Standard library for parsing .env files with similar features to the nodejs
version of dotenv including variable expansion, comments, and multiline values.

<p align="right">(<a href="#top">back to top</a>)</p>

## Features 

- **EnvDocument** works as an ordered dictionary of key/value pairs but can also contain comments and blank lines.
- **Variable Expansion** enables substitution of variables similar to bash shell variables and setting default values.
- **Comments**
- **Backtick Quotes** - allow multi-line values that can use single or double quotes without the need to escape the quotes.
- Multiline Values
- Quoted Values
- Escaped Values

<p align="right">(<a href="#top">back to top</a>)</p>

### TODO

- [ ] Serialization to a .env file.
- [ ] Object mapping to a class or struct.

## Installation

```powershell
dotnet add package Bearz.Text.DotEnv
```

```powershell 
<PackageReference Include="Bearz.Text.DotEnv" Version="*" />
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
