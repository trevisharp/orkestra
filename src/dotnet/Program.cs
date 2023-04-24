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

Text text = @"
variable = 3
if variable > 2: // Test if variable is bigger than 2
    print('ok')
else:
    print('not ok')
    print(':(') // titi
print('end of program')
";

while (text.NextLine())
{
    while (text.NextCharacterLine())
    {
        if (text.Is("//"))
        {
            text.Break();
            break;
        }
    }
    text.PopProcessing();
}
text.PopProcessing();

Console.WriteLine(text);