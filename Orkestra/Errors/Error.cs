/* Author:  Leonardo Trevisan Silio
 * Date:    29/04/2024
 */
namespace Orkestra.Errors;

using Packages;

/// <summary>
/// Base class for Errors
/// </summary>
public class Error : PackageElement
{
    public string Message { get; set; }
    public string Title { get; set; }
    public ErrorType Type { get; set; }

    public Error Clone()
        => new Error
        {
            Message = Message,
            Title = Title,
            Type = Type
        };
}