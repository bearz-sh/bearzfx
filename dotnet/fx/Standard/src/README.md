# Bearz.Standard

## Description

Core Bearz library to enable cross-platform development and automation. The library 
makes it easier to use System.Diagnostics.Process, generate cryptographically secure
passwords, mask secrets, and make it easier to work with low level primitives.

## Features

- A rust like Command object for calling executables.
- Secret generator for creating cryptographically secure passwords.
- Secret masker for stripping out secrets found in strings.
- A Std namespace for variant standard classes like Fs, FsPath, and Env
  which provide an altnerative for System.IO.File, Directory, System.Environment
  with additional methods such as manipulating the path variable.
- Environment variable substitution for bash or windows syntax with `Bearz.Std.Env.Expand()`
- A generic OrderedDictionary&lt;TKey,TValue&gt;

## License

MIT
