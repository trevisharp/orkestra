/* Author:  Leonardo Trevisan Silio
 * Date:    25/06/2024
 */
namespace Orkestra.Processings.Implementations;

/// <summary>
/// A processing for a line comment.
/// </summary>
public class LineCommentProcessing(string commentStarter) : Processing
{
    public string CommentStarter => commentStarter;
    public override Text Process(Text text)
    {
        text.ProcessLines();
        while (text.Next())
        {
            text.ProcessCharacters();
            while (text.Next())
            {
                if (text.Is(commentStarter))
                {
                    text.Discard();
                }
            }
        }
        return text;
    }
}
