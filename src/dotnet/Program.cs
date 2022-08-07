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
Key key_EXPRESSION = Key.CreateKey("EXPRESSION", "//.*?//");
Key key_ID = Key.CreateIdentity("ID", "[A-Za-z_][A-Za-z0-9_]*");

var lex = new LexicalAnalyzer();
lex.Add(key_IF);
lex.Add(key_FOR);
lex.Add(key_INT);
lex.Add(key_EQUAL);
lex.Add(key_DOUBLEDOT);
lex.Add(key_LEFTPARENTHESES);
lex.Add(key_RIGHTPARENTHESES);
lex.Add(key_INTVALUE);
lex.Add(key_ID);

Rule rule_key = Rule.Create("key",
    SubRule.Create(key_KEY, key_ID, key_EQUAL, key_EXPRESSION),
    SubRule.Create(key_CONTEXTUAL, key_KEY, key_EQUAL, key_EXPRESSION)
    );


string code = @"
                    if i == 3:
                        for (int i = 0; i < 10; i++)
               ";


var tokens = lex.Parse(code);

foreach (var token in tokens)
{
    Console.WriteLine(token.Key.Name);
}