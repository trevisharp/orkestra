clear

cd ..\..\res\dotnet\
Write-Host ""
Write-Host "Compilando Orkestra Core" -ForegroundColor Blue
Write-Host ""
dotnet build -c Release

cd ..\..\src\dotnet\
Write-Host ""
Write-Host "Executando Symphony Lang" -ForegroundColor Blue
Write-Host ""
if ($args.Length -ne 0) {
    dotnet run -c $args[0]
}
else {
    dotnet run
}