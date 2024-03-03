using Orkestra;

OrkestraApp.Compile("80 * 3 - 40 * 4 - 8000");

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