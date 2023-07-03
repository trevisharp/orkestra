Key she = Key.CreateKeyword("SHE", "she");
Key eats = Key.CreateKeyword("EATS", "eats");
Key a = Key.CreateKeyword("A", "a");
Key fish = Key.CreateKeyword("FISH", "fish");
Key with = Key.CreateKeyword("WITH", "with");
Key fork = Key.CreateKeyword("FORK", "fork");

var lex = new DefaultLexicalAnalyzer();
lex.AddKeys(new Key[]
{
	she, eats, a, fish, with, fork
});

Rule det = Rule.CreateRule("Det", 
	SubRule.Create(a)
);
Rule n = Rule.CreateRule("N", 
	SubRule.Create(fish),
	SubRule.Create(fork)
);
Rule p = Rule.CreateRule("P", 
	SubRule.Create(with)
);
Rule v = Rule.CreateRule("V",
	SubRule.Create(eats)
);
Rule np = Rule.CreateRule("NP",
	SubRule.Create(det, n),
	SubRule.Create(she)
);
Rule pp = Rule.CreateRule("PP",
	SubRule.Create(p, np)
);
Rule vp = Rule.CreateRule("VP",
	SubRule.Create(eats),
	SubRule.Create(v, np)
);
vp.AddSubRules(
	SubRule.Create(vp, pp)
);
Rule start = Rule.CreateStartRule("S",
	SubRule.Create(np, vp)
);

var analyzer = new ChomskyCYKSyntacticAnalyzer();
analyzer.StartRule = start;
analyzer.Add(det);
analyzer.Add(n);
analyzer.Add(p);
analyzer.Add(v);
analyzer.Add(np);
analyzer.Add(pp);
analyzer.Add(vp);
analyzer.Add(start);

var input = "she eats a fish with a fork";

var tokens = lex.Parse(input);

var tree = analyzer.Parse(tokens);

// string code = 
// @"
// ##########################################################################################################
// #######################################  Pre-processing definition  ######################################

// processing all:
// 	int level = 0
// 	int current = 0
// 	bool emptyline = true

// 	processing line:
// 		emptyline = true
// 		current = 0

// 		processing character:
// 			if character is ""\35"":
// 				discard
// 			if character not is tab and character not is newline and character not is space:
// 				emptyline = false
		
// 		if emptyline:
// 			skip
		
// 		processing character:
//             if character is tab:
//                 current += 4
//             else if character is space:
//                 current += 1
//             else:
//                 break

//     	if current > level + 4:
// 			throw TabulationError

// 		if current > level:
// 			level = current
// 			prepend newline STARTBLOCK
		
// 		append ENDLINE

// 		while level > current:
// 			level -= 4
// 			prepend newline ENDBLOCK
//     while level > current:
//         level -= 4
//         prepend newline ENDBLOCK
// 	append ENDFILE
// ";

// OrkestraApp.Compile(code, args);