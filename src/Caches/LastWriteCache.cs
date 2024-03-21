/* Author:  Leonardo Trevisan Silio
 * Date:    21/03/2024
 */
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Orkestra.Cache;

public class LastWriteCache : Cache
{
    public override async Task<CacheResult<T>> TryGet(string filePath, Func<T> creator = null)
    {
        var cache = getFileCache(file);
        var lastWrite = await getLastWriteCache(cache);
        var currentLastWrite = File.GetLastWriteTime(file);
        if (lastWrite.lastSave == currentLastWrite)
            return true;
        
        // TODO: Save lastWrite
        return false;
    }

    private async Task<LastWriteJson> getLastWriteCache(string cache)
    {
        const string lastWriteCache = "lastWrite.json";
        var cacheFile = Path.Combine(cache, lastWriteCache);
        
        var obj = await openJson<LastWriteJson>(cacheFile);
        return obj;
    }

    private record LastWriteJson(DateTime lastSave);
}