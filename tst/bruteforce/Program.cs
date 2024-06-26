using Orkestra;
using Orkestra.Projects;
using Orkestra.LineInterfaces;
using Orkestra.Processings;

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
    Key SUM = "\\+";
    Key SUB = "\\-";
    Key MUL = "\\*";
    Key DIV = "\\/";
    Key POW = "\\^";
    Key MOD = "\\%";
    Key COMMA = "\\,";
    Key OPENPAR = "\\(";
    Key CLOSEPAR = "\\)";

    Key SUBSET = "subset";
    Key OF = "of";
    Key NAT = "nat";
    Key INT = "int";
    Key RAT = "rat";
    Key REAL = "real";
    Key IS = "is";
    Key AND = "and";
    Key OR = "or";
    Key NOT = "not";
    Key DEFINE = "define";
    Key AS = "as";
    Key CONTAINS = "contains";
    Key IF = "if";
    Key THEN = "then";
    Key FOR = "for";
    Key ALL = "all";
    Key SOME = "some";
    Key IN = "in";
    Key CHECK = "check";
    Key CONSIDERING = "considering";
    Key GIVEN = "given";
    
    Key NUMBER = "-?[0-9][0-9\\\\.]*";
    Key ID = identity("[a-z]+");

    Rule baseset, set, op, exp, exps, value, given, boolean,
        definition, inclusion, checking, cond, condinclusion,
        test, tests, import, item, itens, program, fortype;
    
    Processing lineComment = Processing.LineComment("//");
    
    public BruteForceCompiler()
    {
        op = one(SUM, MUL, SUB, DIV, POW, MOD);

        exp = rule(exp => [
            [ NUMBER ],
            [ NUMBER, op, exp ],
            [ ID ],
            [ ID, op, exp ]
        ]);

        baseset = one(NAT, REAL, RAT, INT);

        set = rule(set => [
            [ baseset ],
            [ SUBSET, OF, set ],
            [ OPENPAR, set, COMMA, set, CLOSEPAR ],
            [ ID ]
        ]);

        exps = many(exp, COMMA);

        value = rule(
            [ exp ],
            [ OPENPAR, exps, CLOSEPAR ]
        );

        boolean = rule(
            [ value, IS, value ],
            [ ID, CONTAINS, value ]
        );

        cond = rule(cond => [
            [ boolean ],
            [ boolean, AND, cond ],
            [ boolean, OR, cond ],
            [ NOT, boolean ],
            [ OPENPAR, boolean, CLOSEPAR ]
        ]);

        definition = rule(DEFINE, ID, AS, set);

        inclusion = rule(
            [ ID, CONTAINS, value ],
            [ ID, CONTAINS, ID ]
        );

        condinclusion = rule(IF, cond, THEN, inclusion);

        given = rule(GIVEN, ID, IN, set);

        fortype = one(SOME, ALL);

        test = rule(FOR, fortype, ID, IN, set);
        
        tests = many(test);

        checking = rule(
            [ CHECK, IF, inclusion ],
            [ CHECK, IF, tests, inclusion ]
        );

        import = rule(CONSIDERING, ID);

        item = one(
            definition, inclusion, 
            condinclusion, given, import
        );

        itens = many(item);

        program = start(
            [ itens ],
            [ itens, checking ]
        );
    }
}