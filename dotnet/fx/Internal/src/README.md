# Common

The Common library shall not be directly used 
or referenced by any other library other than for testing purposes.

The code must be linked as needed by a given assembly.

```xml
<ItemGroup>
  <Compile Include="..\..\Common\src\StringExtensions.cs" Link="StringExtensions..cs" />
</ItemGroup>
```
