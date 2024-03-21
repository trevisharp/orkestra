/* Author:  Leonardo Trevisan Silio
 * Date:    21/03/2024
 */
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Orkestra.Cache;

/// <summary>
/// Represents a Cache that can be updated.
/// </summary>
public abstract class DynamicCache<T> : Cache<T>
{
    /// <summary>
    /// Set the data of cache.
    /// </summary>
    public abstract Task Set(string filePath, T obj);
}