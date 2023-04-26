// using System.Linq;

// string code = 
//     @"
//     contextual key KEY = /key/
//     key CONTEXTUAL = /contextual/
//     contextual key FOR = /for/
//     ";

// OrkestraCompiler compiler = new OrkestraCompiler();
// compiler.Verbose = args.Contains("-v") || args.Contains("--verbose");

// compiler.Compile(code);

Key ENDFILE = Key.CreateAutoKeyword("ENDFILE");
Key ENDLINE = Key.CreateAutoKeyword("ENDLINE");
Key STARTBLOCK = Key.CreateAutoKeyword("STARTBLOCK");
Key ENDBLOCK = Key.CreateAutoKeyword("ENDBLOCK");

Text text = @"
##########################################################################################################
#######################################  Pre-processing definition  ######################################

processing all:
	int level = 0
	int current = 0
	bool emptyline = true

	processing line:
		emptyline = true
		current = 0

		processing character:
			if character is ""\35"":
				discard
			if character not is tab and character not is newline and character not is space:
				emptyline = false
		
		if emptyline:
			skip
		
		processing character:
            if character is tab:
                current += 4
            else if character is space:
                current += 1
            else:
                break

    	if current > level + 4:
			throw TabulationError

		if current > level:
			level = current
			prepend newline STARTBLOCK
		
		append ENDLINE

		while level > current:
			level -= 4
			prepend newline ENDBLOCK
    while level > current:
        level -= 4
        prepend newline ENDBLOCK
	append ENDFILE
";

int level = 0;
int current = 0;
bool emptyline = true;

while (text.NextLine())
{
    emptyline = true;
    current = 0;

    while (text.NextCharacterLine())
    {
        if (text.Is("#"))
        {
            text.Discard();
            break;
        }

        if (!text.Is("\t") && !text.Is("\n") && !text.Is(" "))
        {
            emptyline = false;
        }
    }
    text.PopProcessing();

    if (emptyline)
    {
        text.Skip();
        continue;
    }

    while (text.NextCharacterLine())
    {
        if (text.Is("\t"))
        {
            current += 4;
        }
        else if (text.Is(" "))
        {
            current += 1;
        }
        else
        {
            break;
        }
    }
    text.PopProcessing();

    if (current > level)
    {
        level = current;
        text.PrependNewline();
        text.Prepend(STARTBLOCK);
        text.Next();
    }

    if (emptyline)
        continue;
    
    text.Append(ENDLINE);

    while (current < level)
    {
        level -= 4;
        text.PrependNewline();
        text.Prepend(ENDBLOCK);
        text.Next();
    }
}
text.PopProcessing();
while (level > 0)
{
    level -= 4;
    text.Append(ENDBLOCK);
    text.AppendNewline();
}
text.Append(ENDFILE);

Console.WriteLine(text);