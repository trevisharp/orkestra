Key key_IF = Key.CreateKeyword("IF", "if");

var lexAnalyzer = new LexicalAnalyzer();
lexAnalyzer.Add(key_IF);

var tokens = lexAnalyzer.Parse("if if if");

foreach (var token in tokens)
{
    Console.WriteLine(token.Key.Name);
}