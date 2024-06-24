using Orkestra;
using Orkestra.Projects;
using Orkestra.LineInterfaces;

CLI.Run(args);

public class BruteForceCLI : CLI
{
    [HelpMessage("Run the BruteForce project.")]
    void run(params string[] args)
    {
        BruteForceProject.Compile(args);
    }

    [HelpMessage("Install the BruteForce extension for VSCode.")]
    void install(params string[] args)
    {
        BruteForceProject.InstallExtension(args);
    }
}

public class BruteForceProject : Project<BruteForceProject>
{
    public BruteForceProject()
    {
        Add<BruteForceCompiler>(
            new FileSelector("*.bf")
        );
    }
}

public class BruteForceCompiler : Compiler
{
    Key SUM = key("SUM", "\\+");
    Key SUB = key("SUB", "\\-");
    Key MUL = key("MUL", "\\*");
    Key DIV = key("DIV", "\\/");
    Key POW = key("POW", "\\^");
    Key MOD = key("MOD", "\\%");
    Key COMMA = key("COMMA", "\\,");
    Key OPENPAR = key("OPENPAR", "\\(");
    Key CLOSEPAR = key("CLOSEPAR", "\\)");

    Key SUBSET = keyword("subset");
    Key OF = keyword("of");
    Key NAT = keyword("nat");
    Key INT = keyword("int");
    Key RAT = keyword("rat");
    Key REAL = keyword("real");
    Key IS = keyword("is");
    Key AND = keyword("and");
    Key OR = keyword("or");
    Key NOT = keyword("not");
    Key DEFINE = keyword("define");
    Key AS = keyword("as");
    Key CONTAINS = keyword("contains");
    Key IF = keyword("if");
    Key THEN = keyword("then");
    Key FOR = keyword("for");
    Key ALL = keyword("all");
    Key SOME = keyword("some");
    Key IN = keyword("in");
    Key CHECK = keyword("check");
    Key CONSIDERING = keyword("considering");
    Key GIVEN = keyword("given");
    
    Key NUMBER = key("NUMBER", "-?[0-9][0-9\\.]*");
    Key ID = identity("IDENT", "[a-z]+");

    Rule baseset, set, op, exp, exps, value, given, boolean,
        definition, inclusion, checking, cond, condinclusion,
        test, tests, import, item, itens, program, fortype;
    
    public BruteForceCompiler()
    {
        op = Rule.CreateRule(null,
            sub(SUM), sub(MUL), sub(SUB), 
            sub(DIV), sub(POW), sub(MOD)
        );

        exp = Rule.CreateRule("exp");
        exp.AddSubRules(
            sub(NUMBER),
            sub(NUMBER, op, exp),
            sub(ID),
            sub(ID, op, exp)
        );

        baseset = Rule.CreateRule("baseset",
            sub(NAT), sub(REAL), sub(RAT), sub(INT)
        );

        set = Rule.CreateRule("set");
        set.AddSubRules(
            sub(baseset), sub(SUBSET, OF, set), sub(ID)
        );

        exps = many(exp, COMMA);

        value = Rule.CreateRule("value",
            sub(exp),
            sub(OPENPAR, exps, CLOSEPAR)
        );

        boolean = Rule.CreateRule("boolean",
            sub(value, IS, value),
            sub(ID, CONTAINS, value)
        );

        cond = Rule.CreateRule("cond");
        cond.AddSubRules(
            sub(boolean),
            sub(boolean, AND, cond),
            sub(boolean, OR, cond),
            sub(NOT, boolean),
            sub(OPENPAR, boolean, CLOSEPAR)
        );

        definition = Rule.CreateRule("definition",
            sub(DEFINE, ID, AS, set)
        );

        inclusion = Rule.CreateRule("inclusion",
            sub(ID, CONTAINS, value),
            sub(ID, CONTAINS, ID)
        );

        condinclusion = Rule.CreateRule("condinclusion",
            sub(IF, cond, THEN, inclusion)
        );

        given = Rule.CreateRule("given",
            sub(GIVEN, ID, IN, set)
        );

        fortype = Rule.CreateRule("fortype",
            sub(SOME),
            sub(ALL)
        );

        test = Rule.CreateRule("test",
            sub(FOR, fortype, ID, IN, set)
        );
        
        tests = Rule.CreateRule("tests");
        tests.AddSubRules(
            sub(test),
            sub(test, tests)
        );

        checking = Rule.CreateRule("cheking",
            sub(CHECK, IF, inclusion),
            sub(CHECK, IF, tests, inclusion)
        );

        import = Rule.CreateRule("import",
            sub(CONSIDERING, ID)
        );

        item = Rule.CreateRule("item",
            sub(definition),
            sub(inclusion),
            sub(condinclusion),
            sub(given),
            sub(import)
        );

        itens = Rule.CreateRule("itens");
        itens.AddSubRules(
            sub(item),
            sub(item, itens)
        );

        program = Rule.CreateStartRule("program",
            sub(itens),
            sub(itens, checking)
        );
    }
}