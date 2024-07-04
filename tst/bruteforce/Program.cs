using Orkestra;
using Orkestra.Processings;

Tech.ConfigureProject("*.bf");
Tech.Run(args);

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
    
    Processing lineComment = Processing.LineComment("::");
    
    public BruteForceCompiler()
    {
        op = SUM | MUL | SUB | DIV | MOD | POW;

        exp = rule(exp => [
            [ NUMBER ],
            [ NUMBER, op, exp ],
            [ ID ],
            [ ID, op, exp ]
        ]);

        baseset = NAT | REAL | INT | RAT;

        set = rule(set => [
            [ baseset ],
            [ SUBSET, OF, set ],
            [ OPENPAR, set, COMMA, set, CLOSEPAR ],
            [ OPENPAR, set, COMMA, set, COMMA, set, CLOSEPAR ],
            [ ID ]
        ]);

        exps = many(exp, COMMA);

        value = [
            [ exp ],
            [ OPENPAR, exps, CLOSEPAR ]
        ];

        boolean = [
            [ value, IS, value ],
            [ ID, CONTAINS, value ]
        ];

        cond = rule(cond => [
            [ boolean ],
            [ boolean, AND, cond ],
            [ boolean, OR, cond ],
            [ NOT, boolean ],
            [ OPENPAR, boolean, CLOSEPAR ]
        ]);

        definition = [[DEFINE, ID, AS, set]];

        inclusion = [
            [ ID, CONTAINS, value ],
            [ ID, CONTAINS, ID ]
        ];

        condinclusion = [[IF, cond, THEN, inclusion]];

        given = [[GIVEN, ID, IN, set]];

        fortype = SOME | ALL;

        test = [[FOR, fortype, ID, IN, set]];
        
        tests = many(test);

        checking = [
            [ CHECK, IF, inclusion ],
            [ CHECK, IF, tests, inclusion ]
        ];

        import = [[ CONSIDERING, ID ]];

        item = definition | inclusion | condinclusion | given | import;

        itens = many(item);

        program = start(
            [ itens ],
            [ itens, checking ]
        );
    }
}