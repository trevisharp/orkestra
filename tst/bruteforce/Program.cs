using Orkestra;
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

public class MyWebCLI : CLI
{
    [HelpMessage("Run web app.")]
    void run(params string[] args) {
        MyWebProject.Compile(args);
    }

    [HelpMessage("Run tests web app.")]
    void test(params string[] args) {
        // run tests file
    }

    [HelpMessage("Add a new item to the web app.")]
    void add(params string[] args) {
        if (args[0] == "page") {
            // Creata new .myhtml
        }
    }
}

public class MyWebProject : Project<MyWebProject>
{
    public MyWebProject()
    {
        Add<MyJSCompiler>(
            new FileSelector("*.myjs")
        );
        Add<MyHTMLCompiler>(
            new FileSelector("*.myhtml")
        );
        Add<MyConfigCompiler>(
            new FileSelector("configuration.json")
        );
    }
}

public class MyConfigCompiler : Compiler;
public class MyJSCompiler : Compiler;
public class MyHTMLCompiler : Compiler;

public class BruteForceCompiler : Compiler
{
    // Key SUM = key("SUM", "\\+");
    // Key SUB = key("SUB", "\\-");
    // Key MUL = key("MUL", "\\*");
    // Key DIV = key("DIV", "\\/");
    // Key POW = key("POW", "\\^");
    // Key COMMA = key("COMMA", "\\,");
    // Key OPENPAR = key("OPENPAR", "\\(");
    // Key CLOSEPAR = key("CLOSEPAR", "\\)");

    Key SUBSET = keyword("subset");
    Key OF = keyword("of");
    Key NAT = keyword("nat");
    Key INT = keyword("int");
    Key RAT = keyword("rat");
    Key REAL = keyword("real");
    // Key IS = keyword("is");
    // Key AND = keyword("and");
    // Key OR = keyword("or");
    // Key NOT = keyword("not");
    Key DEFINE = keyword("define");
    Key AS = keyword("as");
    // Key CONTAINS = keyword("contains");
    // Key IF = keyword("if");
    // Key THEN = keyword("then");
    // Key FOR = keyword("for");
    // Key ALL = keyword("all");
    // Key SOME = keyword("some");
    // Key IN = keyword("in");
    // Key CHECK = keyword("check");
    // Key CONSIDERING = keyword("considering");
    // Key GIVEN = keyword("given");
    
    // Key NUMBER = key("NUMBER", "-?[0-9][0-9\\.]+");
    Key ID = identity("IDENT", "[a-z]+");

    Rule set, //op, exp, list, value, boolean, given
        definition, //cond, inclusion, condinclusion,
        //forall, exists, test, tests, checking, import,
        item, itens, program;
    
    public BruteForceCompiler()
    {
        // op = rule("op",
        //     sub(SUM), sub(MUL), sub(SUB), sub(DIV), sub(POW)
        // );

        set = rule("set");
        set.AddSubRules(
            sub(NAT), sub(REAL), sub(RAT), sub(INT),
            sub(SUBSET, OF, set)
        );

        definition = rule("definition");
        definition.AddSubRules(
            sub(DEFINE, ID, AS, set)
        );

        item = rule("item");
        item.AddSubRules(
            sub(definition)
        );

        itens = rule("itens");
        itens.AddSubRules(
            sub(item),
            sub(item, itens)
        );

        program = Rule.CreateStartRule("program");
        program.AddSubRules(
            sub(itens)
        );
    }
}