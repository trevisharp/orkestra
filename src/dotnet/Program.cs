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
	char tabulationtype = 'x'

	processing line:
		emptyline = true
		current = 0
		tabulationtype = 'x'

		processing character:
			if character is ""\35"":
				break
			if character not is tab and character not is newline and character not is space:
				emptyline = false
		
		if emptyline:
			discard
		
		processing character:
			if tabulationtype is 'x':
				if character is tab or character is space:
					tabulationtype = character
				else:
					complete
			
			if character not is tabulationtype:
				complete
			
			if character is tab:
				current += 2
			else if character is space:
				current += 1
		
		if current > level + 2:
			throw TabulationError

		if current > level:
			level = current
			prepend STARTBLOCK
		
		while level > current:
			level -= 2
			append ENDBLOCK
		
		append ENDLINE
	append ENDFILE
";

int level = 0;
int current = 0;
bool emptyline = true;
string tabulationtype = "x";

while (text.NextLine())
{   
    emptyline = true;
    current = 0;
    tabulationtype = "x";

    while (text.NextCharacterLine())
    {
        if (text.Is("#"))
        {
            text.Break();
            break;
        }

        if (!text.Is("\t") && !text.Is("\n") && !text.Is(" ") && !text.Is("\r"))
        {
            emptyline = false;
        }
    }
    text.PopProcessing();

    if (emptyline)
    {
        text.Discard();
        continue;
    }

    while (text.NextCharacterLine())
    {
        if (tabulationtype == "x")
        {
            if (text.Is("\t"))
            {
                current += 2;
                tabulationtype = text;
            }
            else if (text.Is(" "))
            {
                current += 1;
                tabulationtype = text;
            }
            else
            {
                text.Complete();
                break;
            }
        }
        else if (text.Is("\t"))
        {
            current += 2;
        }
        else if (text.Is(" "))
        {
            current += 1;
        }
        else
        {
            text.Complete();
            break;
        }
    }
    text.PopProcessing();

    if (current > level)
    {
        level = current;
        text.PrependNewline();
        text.Prepend(STARTBLOCK);
        text.Skip();
    }

    if (emptyline)
        continue;
    
    text.Append(ENDLINE);

    while (current < level)
    {
        level -= 2;
        text.AppendNewline();
        text.Append(ENDBLOCK);
    }
}
text.PopProcessing();
text.Append(ENDFILE);


Console.WriteLine(text);