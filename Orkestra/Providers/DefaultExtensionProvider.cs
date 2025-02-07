/* Author:  Leonardo Trevisan Silio
 * Date:    11/06/2023
 */
namespace Orkestra.Providers;

using Extensions;
using Extensions.VSCode;

public class DefaultExtensionProvider : IExtensionProvider
{
    public Extension Provide() => new VSCodeExtension();
}