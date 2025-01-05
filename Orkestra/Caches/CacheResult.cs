/* Author:  Leonardo Trevisan Silio
 * Date:    26/11/2024
 */
namespace Orkestra.Caches;

/// <summary>
/// Return if a Cache hit the data or not and return the store object. 
/// </summary>
public record CacheResult<T>(bool IsHit, T? Object)
{
    /// <summary>
    /// Get a cache hit with value.
    /// </summary>
    public static CacheResult<T> Hit(T obj) => new (true, obj);

    /// <summary>
    /// Get a cache Miss without value.
    /// </summary>
    public readonly static CacheResult<T> Miss = new(false, default);
}