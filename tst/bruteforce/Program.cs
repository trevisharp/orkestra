﻿using Orkestra;
using Orkestra.LineInterfaces;
using Orkestra.Projects;

CLI.Run(args);

public class BruteForceCLI : CLI
{
    [HelpMessage("Run the BruteForce project.")]
    void run(params string[] args)
    {
        BruteForceProject.Compile(args);
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
    
    Key NUMBER = key("NUMBER", "-?[0-9][0-9\\.]+");
    Key ID = identity("IDENT", "[a-z]+");

    Rule set, op, exp, list, value, boolean,
        cond, definition, inclusion, condinclusion,
        forall, exists, test, tests, checking, import,
        given, item, itens, program;
    
    public BruteForceCompiler()
    {
        op = rule("op",
            sub(SUM), sub(MUL), sub(SUB), sub(DIV), sub(POW)
        );

        set = rule("set");
        set.AddSubRules(
            sub(NAT), sub(REAL), sub(RAT), sub(INT),
            sub(SUBSET, OF, set)
        );

        definition = rule("definition");
        set.AddSubRules(
            sub(DEFINE, ID, AS, set)
        );

        item = rule("item");
        item.AddSubRules(
            sub(definition)
        );

        itens = rule("itens");
        itens.AddSubRules(
            sub(itens),
            sub(item, itens)
        );

        program = Rule.CreateStartRule("program");
        program.AddSubRules(
            sub(itens)
        );
    }
}