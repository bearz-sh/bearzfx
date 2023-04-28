shebang := if os() == 'windows' {
  'pwsh.exe'
} else {
  '/usr/bin/env pwsh'
}

sln := "Fx.sln"
root_dir := justfile_directory()
export ROOT_DIR := root_dir
export DOTNET_CONFIGURATION := env_var_or_default("DOTNET_CONFIGURATION", "DEBUG")
export SLN := root_dir + "/" + sln
export ARTIFACTS_DIR := root_dir + "/.artifacts"
export PACKAGES_DIR := root_dir + "/.artifacts/packages"

set dotenv-load

fail: 
    #!{{ shebang }}
    Write-Host "Failed"
    exit 1

restore: 
    #!{{ shebang }}
    dotnet restore $ENV:SLN
    exit $LASTEXITCODE
    
add-migration MIGRATION:
     #!{{ shebang }}
     $env:MIGRATION = "{{ MIGRATION }}"
     $cwd = "{{ justfile_directory() }}"
     $project = "$cwd/Casa.Data/src"
     dotnet-ef migrations add $ENV:MIGRATION -p $project --context CasaDbContext --output-dir Migrations/Sqlite -- sqlite
     dotnet-ef migrations add $ENV:MIGRATION -p $project --context CasaDbContext --output-dir Migrations/Mssql -- mssql
     dotnet-ef migrations add $ENV:MIGRATION -p $project --context CasaDbContext --output-dir Migrations/Pg -- pg

build PROJECT='': 
    #!{{ shebang }}
    $p = "{{ PROJECT }}"
    $p = if($p) { 
        $subDir = "src"
        if($p.EndsWith("Tests")) {
            $subDir = "tests"
        } 
      
        [IO.Path]::Combine($ENV:ROOT_DIR, $p, $subDir, "Bearz.$p.csproj")
     } else { $ENV:SLN }
     
    Write-Host $p

    dotnet build $p --no-restore
    exit $LASTEXITCODE
    
test PROJECT='': 
    #!{{ shebang }}
    $p = "{{ PROJECT }}"
    $p = if($p) { 
        $subDir = "$p"
        if(!$p.EndsWith("Tests")) {
            $p = "$p.Tests"
        } 
      
        [IO.Path]::Combine($ENV:ROOT_DIR, $subDir, "test", "Bearz.$p.csproj")
     } else { $ENV:SLN }
     
     Test-Path $p
     Write-Host $p 

    dotnet test $p --no-build --no-restore
    exit $LASTEXITCODE

pack-local: 
    #!{{ shebang }}
    dotnet pack $ENV:SLN --configuration $ENV:DOTNET_CONFIGURATION -o $ENV:PACKAGES_DIR
    exit $LASTEXITCODE

pack: 
    #!{{ shebang }}
    dotnet pack $ENV:SLN --no-build --no-restore --configuration $ENV:DOTNET_CONFIGURATION -o $ENV:PACKAGES_DIR
    exit $LASTEXITCODE

add-local-nuget-feed: 
    #!{{ shebang }}

    $data = "$HOME/.local/share"
    if($IsWindows) {
        $data = "$HOME/AppData/Local"
    }

    if($IsMacOS) {
        $data = "$HOME/Library/Application Support"
    }

    if(Test-Path "./user.env") {
        just nuget-local-feed --dotenv-path = "./user.env" 
    }

    if (!$ENV:NUGET_LOCAL_FEED) {
        $ENV:NUGET_LOCAL_FEED = "$data/Bearz/nuget/packages"
    }

    $dir = (Resolve-Path $ENV:NUGET_LOCAL_FEED).Path
    if(!Test-Path $dir) {
        New-Item -ItemType Directory -Path $dir
    }

    $sources = $(dotnet nuget list source) -join "`n"
    if($sources -notmatch "bearz-local") {
        dotnet nuget add source $dir -n "bearz-local"
    }

new-console PROJECT: 
    #!{{ shebang }}
    $p = "{{ PROJECT }}"
    Write-host "test"
    Write-host "$p"
    $dest = "./$p/$p/src"
    dotnet new sln -n $p -o "./$p"
    dotnet new console -n $p -o "./$p/$p/src"
    dotnet sln "./$p/$p.sln" add "./$p/$p/src/$p.csproj"

install-templates: 
    #!{{ shebang }}
    $templates = Get-Item "./tpl/*"
    foreach($template in $templates) {
        $templatePath = (Resolve-Path $template).Path
        dotnet new -i $templatePath
    }

uninstall-templates: 
    #!{{ shebang }}
    $templates = Get-Item "./tpl/*"
    foreach($template in $templates) {
        $templatePath = (Resolve-Path $template).Path
        dotnet new -u $templatePath
    }
    
casa:
     #!{{ shebang }}
     $cwd = "{{ justfile_directory() }}"
     $tpl = (Resolve-Path "$cwd/../../docker/mssql").Path
     dotnet run --project Casa/src/Casa.csproj -- compose evaluate "$tpl" --overwrite