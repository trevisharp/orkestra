cd ..\..\res\dotnet\
Write-Host ""
Write-Host "Compilando Orkestra Core" -ForegroundColor Blue
Write-Host ""
dotnet build

cd ..\..\src\dotnet\
Write-Host ""
Write-Host "Executando Orkestra Lang" -ForegroundColor Blue
Write-Host ""
dotnet run