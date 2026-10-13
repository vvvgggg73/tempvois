$ErrorActionPreference = "Stop"

$project = Join-Path $PSScriptRoot "Nocturne.csproj"
$publish = Join-Path $PSScriptRoot "publish"
$compiler = Join-Path ${env:ProgramFiles(x86)} "Inno Setup 6\ISCC.exe"

if (-not (Test-Path $compiler)) {
    throw "Не найден Inno Setup 6. Установите его с https://jrsoftware.org/isdl.php и повторите сборку."
}

dotnet publish $project -c Release -r win-x64 --self-contained true `
    -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -o $publish

if ($LASTEXITCODE -ne 0) {
    throw "Публикация приложения завершилась с ошибкой."
}

& $compiler (Join-Path $PSScriptRoot "Nocturne.iss")
if ($LASTEXITCODE -ne 0) {
    throw "Сборка установщика завершилась с ошибкой."
}

Write-Host "Установщик создан в папке dist."