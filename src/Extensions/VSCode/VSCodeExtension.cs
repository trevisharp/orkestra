/* Author:  Leonardo Trevisan Silio
 * Date:    11/06/2023
 */
using System;
using System.IO;
using System.Threading.Tasks;

namespace Orkestra.Extensions.VSCode;

public class VSCodeExtension : Extension
{
    public override Task Generate(ExtensionArguments args)
    {
        throw new System.NotImplementedException();
    }

    void addPackageJson()
    {
        
    }

    string createTempFolder()
    {
        var path = getRandomTempFile();
        initFolder(path);
        return path;
    }

    string getRandomTempFile()
    {
        var temp = Path.GetTempFileName();
        var guid = Guid.NewGuid().ToString();
        var path = Path.Combine(temp, guid);
        return path;
    }

    void initFolder(string path)
    {
        deleteIfExists(path);
        createFile(path);
    }

    void deleteIfExists(string path)
    {
        if (!Path.Exists(path))
            return;

        var dict = new DirectoryInfo(path);
        dict.Delete(true);
    }

    void createFile(string path)
        => Directory.CreateDirectory(path);
}