string code = @"contextual key KEY = /key/
                key CONTEXTUAL = /contextual/
                contextual key KEY = /key/";

OrkestraCompiler compiler = new OrkestraCompiler();

compiler.Compile(code);