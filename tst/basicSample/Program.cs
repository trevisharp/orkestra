using Orkestra;
using Orkestra.LineInterfaces;
using Orkestra.Projects;

OrkestraApp.Run(args);

public class BasicSampleCLI : CLI
{
    [HelpMessage("Run app")]
    void run(params string[] args)
    {
        BasicSampleProject.Compile(args);
    }
}

public class BasicSampleProject : Project<BasicSampleProject>
{
    public BasicSampleProject()
    {
        Add<BasicSampleCompiler>(
            new FileSelector("*.mylang")
        );
    }
}

public class BasicSampleCompiler : Compiler
{
    Key kSUB = keyword("SUB", "\\-");
    Key kMUL = keyword("MUL", "\\*");
    Key kIDENT = key("IDENT", "[0-9]+");

    Rule rExp;
    Rule rTerm;
    Rule rFactor;

    public BasicSampleCompiler()
    {
        rFactor = rule("factor");
        rFactor.AddSubRules(sub(kIDENT));
        
        rTerm = rule("term");
        rTerm.AddSubRules(
            sub(rFactor, kMUL, rTerm),
            sub(rFactor)
        );
        
        rExp = Rule.CreateStartRule("expr");
        rExp.AddSubRules(sub(rTerm, kSUB, rExp));
        rExp.AddSubRules(sub(rTerm));
    }
}