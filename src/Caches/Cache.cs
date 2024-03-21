/* Author:  Leonardo Trevisan Silio
 * Date:    21/03/2024
 */
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Orkestra.Caches;

public static class Cache
{
    public readonly static LastWriteCache LastWrite = new LastWriteCache();
}

/// <summary>
/// A base class for all caches of data.
/// </summary>
public abstract class Cache<T>
{
    /// <summary>
    /// Try get a specific data about a file.
    /// </summary>
    public abstract Task<CacheResult<T>> TryGet(string filePath);

    /// <summary>
    /// Set the data of cache.
    /// </summary>
    public abstract Task Set(string filePath, T obj);
    
    /// <summary>
    /// Return if a chache exists based on cache Id.
    /// </summary>
    protected bool exists(string filePath, string cacheId)
    {
        var cacheFolder = getFileCache(filePath);
        var cacheFile = Path.Combine(cacheFolder, cacheId);
        return Path.Exists(cacheFile);
    }

    /// <summary>
    /// Open a json data from a cache of a file based in cacheId and return a object of type T.
    /// </summary>
    protected async Task<J> openJson<J>(string filePath, string cacheId)
    {
        var cacheFolder = getFileCache(filePath);
        var cacheFile = Path.Combine(cacheFolder, cacheId);

        var json = await File.ReadAllTextAsync(cacheFile);
        var obj = JsonSerializer.Deserialize<J>(json);
        return obj;
    }

    /// <summary>
    /// Save a object of type T in a json file of a cache of a file based in cacheId.
    /// </summary>
    protected async Task saveJson<J>(string filePath, string cacheId, J obj)
    {
        var cacheFolder = getFileCache(filePath);
        var cacheFile = Path.Combine(cacheFolder, cacheId);

        var json = JsonSerializer.Serialize<J>(obj);
        await File.WriteAllTextAsync(cacheFile, json);
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
}