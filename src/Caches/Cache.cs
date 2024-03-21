/* Author:  Leonardo Trevisan Silio
 * Date:    19/03/2024
 */
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Orkestra.Cache;

public class Cache(string file)
{
    public Task<bool> LastWriteTest()
    {
        var cache = getFileCache(file);

        
    }

    private async Task<LastWriteJson> getLastWriteCache(string cache)
    {
        const string lastWriteCache = "lastWrite.json";
        var cacheFile = Path.Combine(cache, lastWriteCache);
        
        var obj = await openJson<LastWriteJson>(cacheFile);
        return obj;
    }

    private async Task<T> openJson<T>(string file)
    {
        var json = await File.ReadAllTextAsync(file);
        var obj = JsonSerializer.Deserialize<T>(json);
        return obj;
    }

    private string getFileCache(string filePath)
    {
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var cacheFolder = getCacheFolderPath();
        var fileCache = Path.Combine(cacheFolder, fileName);
        if (Path.Exists(fileCache))
            return fileCache;
        
        Directory.CreateDirectory(fileCache);
        return fileCache;
    }

    private string getDefaultCacheFolderPath()
    {
        var basePath = Environment.CurrentDirectory;
        const string cacheFolder = ".cache";
        return Path.Combine(basePath, cacheFolder);
    }

    private string getCacheFolderPath()
    {
        var cacheFolder = getDefaultCacheFolderPath();
        if (Path.Exists(cacheFolder))
            return cacheFolder;
        
        Directory.CreateDirectory(cacheFolder);
        return cacheFolder;
    }

    private record LastWriteJson(DateTime lastSave);
}