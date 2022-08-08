string code = @"contextual key KEY = /key/
                key CONTEXTUAL = /contextual/";

OrkestraCompiler compiler = new OrkestraCompiler();

compiler.Compile(code);