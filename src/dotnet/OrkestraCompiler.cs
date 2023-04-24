public class OrkestraCompiler : Compiler
{
    Key key_ENDFILE = Key.CreateAutoKeyword("ENDFILE");
    Key key_ENDLINE = Key.CreateAutoKeyword("ENDLINE");
    Key key_STARTBLOCK = Key.CreateAutoKeyword("STARTBLOCK");
    Key key_ENDBLOCK = Key.CreateAutoKeyword("ENDBLOCK");
    
    Key key_IF = Key.CreateKeyword("IF", "if");
    Key key_FOR = Key.CreateKeyword("FOR", "for");
    Key key_INT = Key.CreateKeyword("INT", "int");
    Key key_EQUAL = Key.CreateKeyword("EQUAL", "=");
    Key key_DOUBLEDOT = Key.CreateKeyword("DOUBLEDOT", ":");
    Key key_LEFTPARENTHESES = Key.CreateKeyword("LEFTPARENTHESES", "\\(");
    Key key_RIGHTPARENTHESES = Key.CreateKeyword("RIGHTPARENTHESES", "\\)");
    Key key_INTVALUE = Key.CreateKey("INTVALUE", "(\\+|\\-)?[0-9][0-9]*");
    Key key_KEY = Key.CreateKeyword("KEY", "key");
    Key key_CONTEXTUAL = Key.CreateKeyword("CONTEXTUAL", "contextual");
    Key key_EXPRESSION = Key.CreateKey("EXPRESSION", "\\/.*?\\/");
    Key key_ID = Key.CreateIdentity("ID", "[A-Za-z_][A-Za-z0-9_]*");

    Rule rule_key;
    Rule rule_start;

    Processing processing1;

    public OrkestraCompiler()
    {
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
                return text;
            }
        );
    }
}