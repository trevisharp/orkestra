public class SymphonyCompiler : Compiler
{
    // auto keys
    Key kENDFILE = auto("ENDFILE");
    Key kENDLINE = auto("ENDLINE");
    Key kSTARTBLOCK = auto("STARTBLOCK");
    Key kENDBLOCK = auto("ENDBLOCK");

    // processing keys
    Key kPROCESSING = keyword("PROCESSING", "processing");
    Key kNEWLINE = keyword("NEWLINE", "newline");
    Key kTAB = keyword("TAB", "tab");
    Key kSPACE = keyword("SPACE", "space");
    Key kALL = contextual("all");
    Key kLINE = contextual("line");
    Key kCHARACTER = contextual("character");
    Key kCONTINUE = contextual("continue");
    Key kSKIP = contextual("skip");
    Key kNEXT = contextual("next");
    Key kBREAK = contextual("break");
    Key kDISCARD = contextual("discard");
    Key kAPPEND = contextual("append");
    Key kPREPEND = contextual("prepend");
    Key kREPLACE = contextual("replace");
    
    // type keys
    Key kINT = keyword("int");
    Key kBOOL = keyword("bool");
    Key kSTRING = keyword("string");
    Key kCHAR = keyword("char");
    Key kDECIMAL = keyword("decimal");
    Key kDOUBLE = keyword("double");
    Key kFLOAT = keyword("float");
    Key kANY = keyword("any");
    Key kTUPLE = keyword("tuple");
    Key kLIST = keyword("list");
    Key kMAP = keyword("map");

    // arithmetic keys
    Key kEQUAL = keyword("EQUAL", "=");
    Key kOPSUM = keyword("OPSUM", "\\+");
    Key kOPSUB = keyword("OPSUB", "\\-");
    Key kOPMUL = keyword("OPMUL", "\\*");
    Key kOPDIV = keyword("OPDIV", "\\/");
    Key kOPMOD = keyword("OPMOD", "\\%");
    Key kOPPOW = keyword("OPPOW", "\\^");
    Key kIS = keyword("is");
    Key kIN = keyword("in");
    Key kNOT = keyword("not");
    Key kBIGGEREQUAL = keyword("BIGGEREQUAL", ">=");
    Key kSMALLEREQUAL = keyword("SMALLEREQUAL", "<=");
    Key kBIGGER = keyword("BIGGER", ">");
    Key kSMALLER = keyword("SMALLER", "<");
    Key kAND = keyword("and");
    Key kOR = keyword("or");
    Key kXOR = keyword("xor");

    // code flow keys
    Key kIF = keyword("if");
    Key kELSE = keyword("else");
    Key kFOR = keyword("for");
    Key kWHILE = keyword("while");
    Key kRETURN = keyword("return");
    Key kPRINT = keyword("print");

    // error keywrods
    Key kTHROW = keyword("throw");

    // symbol keys
    Key kOPENPARENTHESES = keyword("OPENPARENTHESES", "\\(");
    Key kCLOSEPARENTHESES = keyword("CLOSEPARENTHESES", "\\)");
    Key kOPENBRACES = keyword("OPENBRACES", "\\{");
    Key kCLOSEBRACES = keyword("CLOSEBRACES", "\\}");
    Key kOPENBRACKETS = keyword("OPENBRACKETS", "\\[");
    Key kCLOSEBRACKETS = keyword("CLOSEBRACKETS", "\\]");

    Key kDOUBLEDOT = keyword("DOUBLEDOT", ":");
    Key kKEY = keyword("key");
    Key kCONTEXTUAL = keyword("contextual");
    Key kNULLVALUE = keyword("null");

    // value keys
    Key kCOMMA = key("COMMA", ",");
    Key kINTVALUE = key("INTVALUE", "(\\+|\\-)?[0-9][0-9]*");
    Key kSTRINGVALUE = key("STRINGVALUE", "\".*?\"");
    Key kBOOLVALUE = key("BOOLVALUE", "(true)|(false)");

    Key kEXPRESSION = key("EXPRESSION", "\\/.*?\\/");
    Key kID = identity("ID", "[A-Za-z_][A-Za-z0-9_]*");

    Rule rKey;
    Rule rIdentity;
    Rule rStart;
    Rule rBasetype;
    Rule rType;
    Rule rOperation;
    Rule rCommand;
    Rule rCommandBlock_Temp1;
    Rule rData;
    Rule rCondition;
    Rule rDataCollection_Temp1;
    Rule rDataCollection;
    Rule rListValue;
    Rule rMapKey;
    Rule rMapKeyCollection;
    Rule rMapValue;
    Rule rTupleValue;
    Rule rTupleValue_Temp1;
    Rule rVariable;
    Rule rCommandBlock;
    Rule rifStructure;
    Rule rIf;
    Rule rElse;
    Rule rElseIf;
    Rule rElseIfCollection;
    Rule rWhile;
    Rule rThorw;
    Rule rProcessingUnity;
    Rule rProcessingCommand;
    Rule rCommandBlock_Temp1_ProcessingCommand;
    Rule rCommandBlock_ProcessingCommand;
    Rule rifStructure_ProcessingCommand;
    Rule rIf_ProcessingCommand;
    Rule rElse_ProcessingCommand;
    Rule rElseIf_ProcessingCommand;
    Rule rElseIfCollection_ProcessingCommand;
    Rule rWhile_ProcessingCommand;
    Rule rProcessing;

    Processing processing1;

    Error TabulationError = new Error()
    {
        Title = "TabulationError"
    };

    public SymphonyCompiler()
    {
        rIdentity = rule("identity",
            sub(kID)
        );

        rBasetype = rule("basetype", 
            sub(kINT),
            sub(kBOOL),
            sub(kSTRING),
            sub(kCHAR),
            sub(kDECIMAL),
            sub(kDOUBLE),
            sub(kFLOAT),
            sub(kANY)
        );

        rType = rule("type",
            sub(rBasetype)
        );
        rType.AddSubRules(
            sub(kTUPLE, rType),
            sub(kLIST, rType),
            sub(kMAP, rBasetype, rType),
            sub(kTUPLE),
            sub(kLIST),
            sub(kMAP, rBasetype),
            sub(rIdentity)
        );

        rOperation = rule("operation",
            sub(kOPSUM),
            sub(kOPSUB),
            sub(kOPMUL),
            sub(kOPDIV),
            sub(kOPMOD),
            sub(kOPPOW)
        );

        rData = rule("data");
        rData.AddSubRules(
            sub(rData, rOperation, rData),
            sub(kOPSUM, rData),
            sub(kOPSUB, rData),
            sub(kINTVALUE),
            sub(kBOOLVALUE),
            sub(kSTRINGVALUE),
            sub(kEXPRESSION),
            sub(kNULLVALUE),
            sub(kNEWLINE),
            sub(kTAB),
            sub(kSPACE)
        );

        rCondition = rule("condition",
            sub(kBOOLVALUE),
            sub(rData, kIS, rData),
            sub(rData, kNOT, kIS, rData),
            sub(rData, kIN, rData),
            sub(rData, kSMALLER, rData),
            sub(rData, kSMALLEREQUAL, rData),
            sub(rData, kBIGGER, rData),
            sub(rData, kBIGGEREQUAL, rData),
            sub(rIdentity)
        );
        rCondition.AddSubRules(
            sub(rCondition, kAND, rCondition),
            sub(rCondition, kOR, rCondition),
            sub(rCondition, kXOR, rCondition),
            sub(kNOT, rCondition),
            sub(kOPENPARENTHESES, rCondition, kCLOSEPARENTHESES)
        );
        rData.AddSubRules(
            sub(rCondition)
        );

        rDataCollection_Temp1 = rule("temp1datacollection");
        rDataCollection_Temp1.AddSubRules(
            sub(rData, kCOMMA, rDataCollection_Temp1),
            sub(rData, kCOMMA)
        );
        rDataCollection = rule("dataCollection",
            sub(rDataCollection_Temp1, rData),
            sub(rData)
        );

        rListValue = rule("listValue",
            sub(kOPENBRACKETS, rDataCollection, kCLOSEBRACKETS),
            sub(kOPENBRACKETS, kCLOSEBRACKETS)
        );

        rMapKey = rule("mapKey",
            sub(rData, kDOUBLEDOT, rData)
        );

        rMapKeyCollection = rule("mapKeyCollection",
            sub(rMapKey, kENDLINE),
            sub(rMapKey)
        );
        rMapKeyCollection.AddSubRules(
            sub(rMapKey, kCOMMA, kENDLINE, rMapKeyCollection),
            sub(rMapKey, kCOMMA, rMapKeyCollection)
        );

        rMapValue = rule("mapValue",
            sub(kOPENBRACES, kENDLINE, kSTARTBLOCK, rMapKeyCollection, kENDBLOCK, kCLOSEBRACES),
            sub(kOPENBRACES, kSTARTBLOCK, rMapKeyCollection, kENDBLOCK, kCLOSEBRACES),
            sub(kOPENBRACES, kENDLINE, kCLOSEBRACES),
            sub(kOPENBRACES, kCLOSEBRACES)
        );

        rTupleValue_Temp1 = rule("temp1tuplevalue");
        rTupleValue_Temp1.AddSubRules(
            sub(rData, rTupleValue_Temp1),
            sub(rData)
        );

        rTupleValue = rule("tupleValue",
            sub(kOPENPARENTHESES, kCLOSEPARENTHESES),
            sub(kOPENPARENTHESES, rTupleValue_Temp1, kCLOSEPARENTHESES)
        );

        rVariable = rule("variable",
            sub(rType, rIdentity),
            sub(rType, rIdentity, kEQUAL, rData)
        );

        rThorw = rule("throw",
            sub(kTHROW, rIdentity)
        );

        rCommand = rule("command",
            sub(rIdentity, kEQUAL, rData, kENDLINE),
            sub(rIdentity, rOperation, kEQUAL, rData, kENDLINE),
            sub(rVariable, kENDLINE),
            sub(kRETURN, rData, kENDLINE),
            sub(kPRINT, rData, kENDLINE),
            sub(kBREAK, kENDLINE),
            sub(kCONTINUE, kENDLINE),
            sub(rThorw, kENDLINE)
        );

        rCommandBlock_Temp1 = rule("temp1commandblock");
        rCommandBlock_Temp1.AddSubRules(
            sub(rCommand, rCommandBlock_Temp1),
            sub(rCommand)
        );

        rCommandBlock = rule("commandBlock",
            sub(kSTARTBLOCK, rCommandBlock_Temp1, kENDBLOCK)
        );

        rIf = rule("if",
            sub(kIF, rCondition, kDOUBLEDOT, kENDLINE, rCommandBlock),
            sub(kIF, rCondition)
        );

        rElseIf = rule("elseif",
            sub(kELSE, kIF, rCondition, kDOUBLEDOT, kENDLINE, rCommandBlock),
            sub(kELSE, kIF)
        );
        
        rElseIfCollection = rule("elseIfCollection");
        rElseIfCollection.AddSubRules(
            sub(rElseIf),
            sub(rElseIf, rElseIfCollection)
        );

        rElse = rule("else",
            sub(kELSE, kDOUBLEDOT, kENDLINE, rCommandBlock),
            sub(kELSE)
        );

        rifStructure = rule("ifStrucuture",
            sub(rIf),
            sub(rIf, rElseIfCollection),
            sub(rIf, rElse),
            sub(rIf, rElseIfCollection, rElse)
        );
        rCommand.AddSubRules(
            sub(rifStructure)
        );

        rWhile = rule("while",
            sub(kWHILE, rCondition, kDOUBLEDOT, kENDLINE, rCommandBlock)
        );

        rProcessingUnity = rule("processingUnity",
            sub(kALL),
            sub(kLINE),
            sub(kCHARACTER)
        );
        rData.AddSubRules(
            sub(rProcessingUnity)
        );

        rProcessingCommand = rule("processingCommand",
            sub(kAPPEND, rData, kENDLINE),
            sub(kPREPEND, rData, kENDLINE),
            sub(kDISCARD, kENDLINE),
            sub(kSKIP, kENDLINE),
            sub(kREPLACE, kENDLINE),
            sub(kNEXT, kENDLINE)
        );

        rCommandBlock_Temp1_ProcessingCommand = rule("temp1commandBlockProcessingCommand");
        rCommandBlock_Temp1_ProcessingCommand.AddSubRules(
            sub(rProcessingCommand, rCommandBlock_Temp1_ProcessingCommand),
            sub(rProcessingCommand)
        );

        rCommandBlock_ProcessingCommand = rule("commandBlockProcessingCommand",
            sub(kSTARTBLOCK, rCommandBlock_Temp1_ProcessingCommand, kENDBLOCK)
        );

        rIf_ProcessingCommand = rule("ifProcessingCommand",
            sub(kIF, rCondition, kDOUBLEDOT, kENDLINE, rCommandBlock_ProcessingCommand),
            sub(kIF, rCondition)
        );

        rElseIf_ProcessingCommand = rule("elseifProcessingCommand",
            sub(kELSE, kIF, rCondition, kDOUBLEDOT, kENDLINE, rCommandBlock_ProcessingCommand),
            sub(kELSE, kIF)
        );

        rElse_ProcessingCommand = rule("elseProcessingCommand",
            sub(kELSE, kDOUBLEDOT, kENDLINE, rCommandBlock),
            sub(kELSE)
        );
        
        rElseIfCollection_ProcessingCommand = rule("elseIfCollectionProcessingCommand");
        rElseIfCollection_ProcessingCommand.AddSubRules(  
            sub(rElseIf_ProcessingCommand),
            sub(rElseIf_ProcessingCommand, rElseIfCollection_ProcessingCommand)
        );

        rifStructure_ProcessingCommand = rule("ifStrucutureProcessingCommand",
            sub(rIf_ProcessingCommand),
            sub(rIf_ProcessingCommand, rElseIfCollection_ProcessingCommand),
            sub(rIf_ProcessingCommand, rElse_ProcessingCommand),
            sub(rIf_ProcessingCommand, rElseIfCollection_ProcessingCommand, rElse_ProcessingCommand)
        );

        rWhile_ProcessingCommand = rule("whileProcessingCommand",
            sub(kWHILE, rCondition, kDOUBLEDOT, kENDLINE, rCommandBlock_ProcessingCommand)
        );

        rProcessingCommand.AddSubRules(
            sub(rIdentity, kEQUAL, rData, kENDLINE),
            sub(rIdentity, rOperation, kEQUAL, rData, kENDLINE),
            sub(rVariable, kENDLINE),
            sub(kRETURN, rData, kENDLINE),
            sub(kPRINT, rData, kENDLINE),
            sub(kBREAK, kENDLINE),
            sub(kCONTINUE, kENDLINE),
            sub(rThorw, kENDLINE),
            sub(rifStructure_ProcessingCommand),
            sub(rWhile_ProcessingCommand)
        );

        rKey = rule("key",
            sub(kCONTEXTUAL, kKEY, kID, kEQUAL, kEXPRESSION),
            sub(kKEY, kID, kEQUAL, kEXPRESSION)
        );

        rProcessing = rule("processing",
            sub(kPROCESSING, rProcessingUnity, kDOUBLEDOT, kENDLINE, rCommandBlock_ProcessingCommand),
            sub(kPROCESSING, rProcessingUnity, kDOUBLEDOT, kENDLINE)
        );
        rProcessingCommand.AddSubRules(
            sub(rProcessing)
        );

        rStart = Rule.CreateStartRule("start");
        rStart.AddSubRules(
            sub(rOperation, rStart),
            sub(rKey, rStart),
            sub(rKey)
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
                        text.Prepend(kSTARTBLOCK);
                        text.Next();
                    }
                    
                    text.Append(kENDLINE);

                    while (current < level)
                    {
                        level -= 4;
                        text.PrependNewline();
                        text.Prepend(kENDBLOCK);
                        text.Next();
                    }
                }
                text.PopProcessing();
                while (level > 0)
                {
                    level -= 4;
                    text.Append(kENDBLOCK);
                    text.AppendNewline();
                }
                text.Append(kENDFILE);
                return text;
            }
        );
    }
}