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
    const string lastWriteCache = "lastWrite.json";

    public override async Task<CacheResult<T>> TryGet(string filePath)
    {
        var lastWriteCache = await openJson<LastWriteJson>(filePath, lastWriteCache);
        var currentLastWrite = File.GetLastWriteTime(filePath);
        return lastWriteCache.lastWriteDate == currentLastWrite ?
            CacheResult<T>.Hit(currentLastWrite) : currentLastWrite.Miss;
    }

    public override async Task Set(string filePath, T obj)
        => await saveJson<LastWriteJson>(filePath, obj);

    record LastWriteJson(DateTime lastWriteDate);
}