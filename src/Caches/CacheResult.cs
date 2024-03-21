/* Author:  Leonardo Trevisan Silio
 * Date:    21/03/2024
 */
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Orkestra.Caches;

/// <summary>
/// Return if a Cache hit the data or not and return the store object. 
/// </summary>
public record CacheResult<T>(
    bool IsHit, T? Object
)
{
    public static CacheResult<T> Hit(T obj) => new (true, obj);
    public readonly static CacheResult<T> Miss = new(false, (T)null);
}