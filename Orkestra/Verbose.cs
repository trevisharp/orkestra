/* Author:  Leonardo Trevisan Silio
 * Date:    25/06/2024
 */
using Color = System.ConsoleColor;
using static System.Console;

namespace Orkestra;

public static class Verbose
{
    public static void Configure(params string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            var crr = args[i];
            if (crr != "--verbose" && crr != "-v")
                continue;
            
            var next =
                i + 1 < args.Length ?
                args[i + 1] : null;
            VerboseLevel = next switch
            {
                "max" => int.MaxValue,
                null  => 1,
                _     => int.TryParse(next, out int level) ? level : 1
            };
            return;
        }
        VerboseLevel = 0;
    }

    public static int VerboseLevel = 0;
    private static string? tabInfo = null;
    private static void message(
        int level,
        object msg,
        Color color,
        bool inline = false
    )
    {
        if (VerboseLevel < level)
            return;
        
        if (inline) Write(" ");
        else WriteLine();

        if (!inline)
            Write(tabInfo);
        
        ForegroundColor = color;
        Write(msg);
        ForegroundColor = Color.Gray;
    }
    public static void StartGroup(int level = 0)
    {
        if (VerboseLevel < level)
            return;
        
        tabInfo ??= "";
        tabInfo += "\t";
    }
    public static void EndGroup(int level = 0)
    {
        if (VerboseLevel < level)
            return;
        
        if (tabInfo is null)
            return;
        WriteLine();
        
        if (tabInfo == "")
        {
            tabInfo = null;
            return;
        }

        tabInfo = tabInfo.Remove(0);
    }
    public static void Info(object text, int level = 0)
        => message(level, text, Color.Blue);
    public static void Content(object text, int level = 0)
        => message(level, text, Color.White);
    public static void InlineContent(object text, int level = 0)
        => message(level, text, Color.White, true);
    public static void Success(object text, int level = 0)
        => message(level, text, Color.Green);
    public static void Warning(object text, int level = 0)
        => message(level, text, Color.Yellow);
    public static void Error(object text, int level = 0)
        => message(level, text, Color.Red);
    public static void NewLine(int level = 0)
    {
        if (VerboseLevel < level)
            return;
        
        WriteLine();
    }
}