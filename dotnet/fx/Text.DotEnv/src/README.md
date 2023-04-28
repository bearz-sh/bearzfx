# Bearz.Text.DotEnv

## Description

A .NET Standard library for parsing .env files with similar features to the nodejs
version of dotenv including variable expansion, comments, and multiline values.

## Features

- **EnvDocument** works as an ordered dictionary of key/value pairs but can also contain comments and blank lines.
- **Variable Expansion** enables substitution of variables similar to bash shell variables and setting default values.
- **Comments**
- **Backtick Quotes** - allow multi-line values that can use single or double quotes without the need to escape the quotes.
- Multiline Values
- Quoted Values
- Escaped Values

```text
# This is a comment
TEST=hello_world

  # this is a comment too
PW=X232dwe)()_+!@

## this is a comment woo hoo
MULTI="1
2
3
  4"
HW="Hello, ${WORD:-world}"
```

```csharp
var envDoc = DotEnvLoader.Parse(new DotEnvLoadOptions {
    Files = new[] {"path/to/.env"}
});

foreach (var kvp in envDoc.ToDictionary()) 
{
    Console.WriteLine($"{key} = {value}");
}

```

### TODO

- [ ] Serialization to a .env file.
- [ ] Object mapping to a class or struct.

## License

MIT