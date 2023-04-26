namespace Orkestra.Errors;

using Packages;

public class Error : PackageElement
{
    public string Message { get; set; }
    public string Title { get; set; }
    public ErrorType Type { get; set; }

    public Error Clone()
    {
        Error error = new Error();
        
        error.Message = Message;
        error.Title = Title;
        error.Type = Type;

        return error;
    }
}