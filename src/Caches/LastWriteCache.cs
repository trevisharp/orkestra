/* Author:  Leonardo Trevisan Silio
 * Date:    21/03/2024
 */
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Orkestra.Caches;

public class LastWriteCache : Cache<DateTime>
{
    const string lastWriteCacheId = "lastWrite.json";

    public override async Task<CacheResult<DateTime>> TryGet(string filePath)
    {
        if (!exists(filePath, lastWriteCacheId))
            return CacheResult<DateTime>.Miss;

        var lastWriteCache = await openJson<LastWriteJson>(filePath, lastWriteCacheId);
        var currentLastWrite = File.GetLastWriteTime(filePath);
        return lastWriteCache.lastWriteDate == currentLastWrite ?
            CacheResult<DateTime>.Hit(currentLastWrite) : CacheResult<DateTime>.Miss;
    }

    public override async Task Set(string filePath, DateTime obj)
        => await saveJson<LastWriteJson>(filePath, lastWriteCacheId, new(obj));

    record LastWriteJson(DateTime lastWriteDate);
}