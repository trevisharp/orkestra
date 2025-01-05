/* Author:  Leonardo Trevisan Silio
 * Date:    11/06/2023
 */
namespace Orkestra.Providers;

using Extensions;

/// <summary>
/// A provider for auto extensions generators.
/// </summary>
public interface IExtensionProvider
{
    Extension Provide();
}