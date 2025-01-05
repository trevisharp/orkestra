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
    public readonly static LastWriteCache LastWrite = new();
}

/// <summary>
/// A base class for all caches of data.
/// Recive the id of the cache implementation.
/// The filename of caches are equal to cacheId.
/// </summary>
public abstract class Cache<T>(string cacheId)
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
    protected bool Exists(string id)
    {
        var cacheFile = GetCacheFile(id);
        return Path.Exists(cacheFile);
    }

    /// <summary>
    /// Open a json data based on id and CacheId implementation.
    /// Consider use the name of the file associated with this cache was id.
    /// </summary>
    protected async Task<J?> Load<J>(string id)
    {
        var cacheFile = GetCacheFile(id);
        var json = await File.ReadAllTextAsync(cacheFile);
        var obj = JsonSerializer.Deserialize<J>(json);
        return obj;
    }

    /// <summary>
    /// Store a json data based on id and CacheId implementation.
    /// Consider use the name of the file associated with this cache was id.
    /// </summary>
    protected async Task Store<J>(string id, J obj)
    {
        var cacheFile = GetCacheFile(id);
        var json = JsonSerializer.Serialize(obj);
        await File.WriteAllTextAsync(cacheFile, json);
    }
    
    private string GetCacheFile(string id)
    {
        var cacheFolder = GetCacheFolderWithId(id);
        var cacheFile = Path.Combine(cacheFolder, cacheId);
        return cacheFile;
    }

    private static string GetCacheFolderWithId(string id)
    {
        var folderName = Path.GetFileNameWithoutExtension(id);
        var cacheFolder = GetCacheFolderPath();
        var fileCache = Path.Combine(cacheFolder, folderName);
        if (Path.Exists(fileCache))
            return fileCache;
        
        Directory.CreateDirectory(fileCache);
        return fileCache;
    }

    private static string GetCacheFolderPath()
    {
        var cacheFolder = GetDefaultCacheFolderPath();
        if (Path.Exists(cacheFolder))
            return cacheFolder;
        
        Directory.CreateDirectory(cacheFolder);
        return cacheFolder;
    }
    
    private static string GetDefaultCacheFolderPath()
    {
        const string cacheFolder = ".cache";
        var basePath = Environment.CurrentDirectory;
        return Path.Combine(basePath, cacheFolder);
    }
}