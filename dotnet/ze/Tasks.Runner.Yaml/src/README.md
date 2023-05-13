# Ze.Tasks.Runner.Yaml
<a name="top"></a>

## Description

<p align="right">(<a href="#top">back to top</a>)</p>

## Features 

<p align="right">(<a href="#top">back to top</a>)</p>

```yaml
name: task-set-name
include:
  - path: ../tasks.yml
  - name: dotnet # pulls from global files

vaults:
  - vault: dotenv
    path: .env
    sops: true
    prefix: "DOTENV_"
    
vars:
  config:
    one: "one"
    
env:
  NAME: "{{ env.name }}/test"

tasks:
  - name: task1
    run: |
      echo "task1"
    needs: ["task2"]
  - name: task2
    run: |
      echo "task2"
```

## Installation

```powershell
dotnet add package Ze.Tasks.Runner.Yaml
```

```powershell 
<PackageReference Include="Ze.Tasks.Runner.Yaml" Version="*" />
```

<p align="right">(<a href="#top">back to top</a>)</p>

## License 

{{ license }}

<p align="right">(<a href="#top">back to top</a>)</p>
