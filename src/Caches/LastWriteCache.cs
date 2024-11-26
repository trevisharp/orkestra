/* Author:  Leonardo Trevisan Silio
 * Date:    26/11/2024
 */
using System;
using System.IO;
using System.Threading.Tasks;

namespace Orkestra.Caches;

public class LastWriteCache() : Cache<DateTime>("lastWrite.json")
{
    public override async Task<CacheResult<DateTime>> TryGet(string filePath)
    {
        if (!Exists(filePath))
            return CacheResult<DateTime>.Miss;

        var lastWriteCache = await Load<LastWriteJson>(filePath);
        if (lastWriteCache is null)
            return CacheResult<DateTime>.Miss;

        var currentLastWrite = File.GetLastWriteTime(filePath);
        return lastWriteCache.LastWriteDate == currentLastWrite ?
            CacheResult<DateTime>.Hit(currentLastWrite) : CacheResult<DateTime>.Miss;
    }

    public override async Task Set(string filePath, DateTime obj)
        => await Store<LastWriteJson>(filePath, new(obj));

    record LastWriteJson(DateTime LastWriteDate);
}