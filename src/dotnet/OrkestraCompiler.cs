public class OrkestraCompiler : Compiler
{
    // auto keys
    Key key_ENDFILE = Key.CreateAutoKeyword("ENDFILE");
    Key key_ENDLINE = Key.CreateAutoKeyword("ENDLINE");
    Key key_STARTBLOCK = Key.CreateAutoKeyword("STARTBLOCK");
    Key key_ENDBLOCK = Key.CreateAutoKeyword("ENDBLOCK");

    // processing keys
    Key key_PROCESSING = key("PROCESSING", "processing");
    Key NEWLINE = key("NEWLINE", "newline");
    Key key_TAB = key("TAB", "tab");
    Key key_SPACE = key("SPACE", "space");
    Key key_ALL = contextual("ALL", "all");
    Key key_LINE = contextual("LINE", "line");
    Key key_CHARACTER = contextual("CHARACTER", "character");
    Key key_CONTINUE = contextual("CONTINUE", "continue");
    Key key_SKIP = contextual("SKIP", "skip");
    Key key_NEXT = contextual("NEXT", "next");
    Key key_BREAK = contextual("BREAK", "break");
    Key key_DISCARD = contextual("DISCARD", "discard");
    Key key_APPEND = contextual("APPEND", "append");
    Key key_PREPEND = contextual("PREPREND", "prepend");
    Key key_REPLACE = contextual("REPLACE", "replace");
    
    // type keys
    Key key_INT = keyword("INT", "int");
    Key key_BOOL = keyword("BOOL", "bool");
    Key key_STRING = keyword("STRING", "string");
    Key key_CHAR = keyword("CHAR", "char");
    Key key_DECIMAL = keyword("DECIMAL", "decimal");
    Key key_DOUBLE = keyword("DOUBLE", "double");
    Key key_FLOAT = keyword("FLOAT", "float");
    Key key_ANY = keyword("ANY", "any");
    Key key_TUPLE = keyword("TUPLE", "tuple");
    Key key_LIST = keyword("LIST", "list");
    Key key_MAP = keyword("MAP", "map");

    // arithmetic keys
    Key key_EQUAL = keyword("EQUAL", "=");
    Key key_OPSUM = keyword("OPSUM", "\\+");
    Key key_OPSUB = keyword("OPSUB", "\\-");
    Key key_IS = keyword("IS", "is");
    Key key_NOT = keyword("NOT", "not");
    Key BIGGEREQUAL = keyword("BIGGEREQUAL", ">=");
    Key SMALLEREQUAL = keyword("SMALLEREQUAL", "<=");
    Key BIGGER = keyword("BIGGER", ">");
    Key SMALLER = keyword("SMALLER", "<");

    // code flow keys
    Key key_IF = keyword("IF", "if");
    Key key_FOR = keyword("FOR", "for");
    Key key_WHILE = keyword("WHILE", "while");


    Key key_DOUBLEDOT = keyword("DOUBLEDOT", ":");
    Key key_LEFTPARENTHESES = keyword("LEFTPARENTHESES", "\\(");
    Key key_RIGHTPARENTHESES = keyword("RIGHTPARENTHESES", "\\)");
    Key key_KEY = keyword("KEY", "key");
    Key key_CONTEXTUAL = keyword("CONTEXTUAL", "contextual");

    // value keys
    Key key_INTVALUE = key("INTVALUE", "(\\+|\\-)?[0-9][0-9]*");
    Key key_BOOLVALUE = key("BOOLVALUE", "(true)|(false)");


    Key key_EXPRESSION = key("EXPRESSION", "\\/.*?\\/");
    Key key_ID = Key.CreateIdentity("ID", "[A-Za-z_][A-Za-z0-9_]*");

    Rule rule_key;
    Rule rule_identity;
    Rule rule_start;
    Rule rule_basetype;
    Rule rule_type;
    Rule rule_command;

    Processing processing1;

    Error TabulationError = new Error()
    {
        Title = "TabulationError"
    };

    public OrkestraCompiler()
    {
        rule_identity = Rule.CreateRule("identity",
            SubRule.Create(key_ID)
        );

        rule_basetype = Rule.CreateRule("basetyle", 
            SubRule.Create(key_INT),
            SubRule.Create(key_BOOL),
            SubRule.Create(key_STRING),
            SubRule.Create(key_CHAR),
            SubRule.Create(key_DECIMAL),
            SubRule.Create(key_DOUBLE),
            SubRule.Create(key_FLOAT),
            SubRule.Create(key_ANY)
        );

        rule_type = Rule.CreateRule("type",
            SubRule.Create(rule_basetype)
        );
        rule_type.AddSubRules(
            SubRule.Create(key_TUPLE, rule_type),
            SubRule.Create(key_LIST, rule_type),
            SubRule.Create(key_MAP, rule_basetype, rule_type),
            SubRule.Create(key_TUPLE),
            SubRule.Create(key_LIST),
            SubRule.Create(key_MAP, rule_basetype),
            SubRule.Create(rule_identity)
        );

        rule_command = new Rule.CreateRule(

        );


        rule_key = Rule.CreateRule("key",
            SubRule.Create(key_CONTEXTUAL, key_KEY, key_ID, key_EQUAL, key_EXPRESSION),
            SubRule.Create(key_KEY, key_ID, key_EQUAL, key_EXPRESSION)
        );

        rule_start = Rule.CreateStartRule("start");
        rule_start.AddSubRules(
            SubRule.Create(rule_key, rule_start),
            SubRule.Create(rule_key)
        );

        processing1 = Processing.FromFunction(
            text =>
            {
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
                    
                    if (current > level + 4)
                        ErrorQueue.Main.Enqueue(TabulationError);

                    if (current > level)
                    {
                        level = current;
                        text.PrependNewline();
                        text.Prepend(key_STARTBLOCK);
                        text.Next();
                    }
                    
                    text.Append(key_ENDLINE);

                    while (current < level)
                    {
                        level -= 4;
                        text.PrependNewline();
                        text.Prepend(key_ENDBLOCK);
                        text.Next();
                    }
                }
                text.PopProcessing();
                while (level > 0)
                {
                    level -= 4;
                    text.Append(key_ENDBLOCK);
                    text.AppendNewline();
                }
                text.Append(key_ENDFILE);
                return text;
            }
        );
    }
}