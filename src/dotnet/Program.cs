string code = @"contextual key KEY = /key/
                key CONTEXTUAL = /contextual/
                contextual key FOR = /for/";

OrkestraCompiler compiler = new OrkestraCompiler();

compiler.Compile(code);