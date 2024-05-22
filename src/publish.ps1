$csproj = gc .\Orkestra.csproj
$versionText = $csproj | % {
    if ($_.Contains("PackageVersion"))
    {
        $_
    }
}

$version = ""
$flag = 0
for ($i = 0; $i -lt $versionText.Length; $i++)
{
    $char = $versionText[$i]

    if ($flag -eq 1)
    {
        if ($char -eq "<")
        {
            break
        }

        $version += $char
    }

    if ($char -eq ">")
    {
        $flag = 1
    }
}

dotnet pack -c Release
$file = ".\bin\Release\Orkestra." + $version + ".nupkg"
cp $file Orkestra.nupkg

$key = gc .\.env

dotnet nuget push Orkestra.nupkg --api-key $key --source https://api.nuget.org/v3/index.json
rm .\Orkestra.nupkg