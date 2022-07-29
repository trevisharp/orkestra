##########################################################################################################
#######################################  Pre-processing definition  ######################################

processing all:
	int level = 0
	int current = 0
	bool emptyline = true
	char tabulationtype = 'x'
	processing line:
		emptyline = true
		current = 0
		tabulationtype = 'x'

		processing character:
			if character is "#":
				break
			if character not is tab and character not is newline and character not is space:
				emptyline = false
		if emptyline:
			jump
		
		processing character:
			if tabulationtype is 'x':
				if character is tab or character is space:
					tabulationtype = character
				else:
					complete
			if character not is tabulationtype:
				complete
			if character is tab:
				current += 2
			else if character is space:
				current += 1
		
		#TODO: tratar espaçamentos exagerados como erros
		processing line:
			if current > level:
				level = current
				return "@startblock" + newline + line
		
		while level > current:
			level -= 2
			processing line:
				return "@endblock" + newline + line

		if emptyline:
			continue
		return line + " @endline"
	return all + " @endfile"

##########################################################################################################
############################################  Key definition  ############################################

key AS = /as/

key PROCESSING = /processing/
contextual key ALL = /all/
contextual key LINE = /line/
contextual key CHARACTER = /character/
contextual key BREAK = /break/
contextual key COMPLETE = /complete/
contextual key JUMP = /jump/
contextual key CONTINUE = /continue/
contextual key SKIP = /skip/
contextual key NEXT = /next/

key ENDFILE = /@endfile/
key ENDLINE = /@endline/
key STARTBLOCK = /@startBlock/
key ENDBLOCK = /@endBlock/
key BRACES = /\{/
key BRACES_CLOSE = /\}/
key PARENTHESES = /\(/
key PARENTHESES_CLOSE = /\)/
key BRACKETS = /\[/
key BRACKETS_CLOSE = /\]/
key OPSUM = /\+/
key OPSUBTRACT = /\-/
key OPTIMES = /\*/
key OPPOW = /\^/
key OPMODULE = /\%/
key EQUAL = /\=/
key DOUBLEDOT = /\:/
key DOT = /\./
key COMMA = /,/
key LAMBDA = /=>/
contextual key IF = /if/
contextual key ELSE = /else/
contextual key FOR = /for/
contextual key WHILE = /while/
key IN = /in/
key IS = /is/
key NOT = /not/
key BIGGEREQUAL = />=/
key SMALLEREQUAL = /<=/
key BIGGER = />/
key SMALLER = /</
key AND = /and/
key OR = /or/
key XOR = /xor/
key INT = /int/
key STRING = /string/
key DECIMAL = /decimal/
key BOOL = /bool/
key CHAR = /char/
key ANY = /any/
key LIST = /list/
key MAP = /map/
key TUPLE = /tuple/
key DOCTREE = /doctree/
contextual key KEY = /key/
key NULLVALUE = /null/
key NEW = /new/
key OVERRIDE = /override/
contextual key ENTITY = /enity/
contextual key PARTIAL = /partial/
contextual key FULL = /full/
key THIS = /this/
key BASE = /base/
contextual key FUNCTION = /function/
contextual key METHOD = /method/
key RETURN = /return/
key INHERITS = /inherits/
contextual key PROPERTY = /property/
key READONLY = /readonly/
key GET = /get/
key SET = /set/
key VALUE = /value/
contextual key CONSTRUCTOR = /constructor/
contextual key RULE = /rule/
contextual key COMPILER = /compiler/
key START = /start/
contextual key FOLDER = /folder/
contextual key FILE = /file/
contextual key TITLE = /title/
contextual key CONTENT = /content/
key CONTEXTUAL = /contextual/
key OPEN = /open/
key PRINT = />>/
contextual key PARSE = /parse/
key NEWLINE = /newline/
key TAB = /tab/
key SPACE = /space/
contextual key USING = /using/
contextual key INCLUDE = /include/
key IDENTITY = /identity/
identity key ID = /[A-Za-z_][A-Za-z_0-9]*/
key STRINGVALUE = /\".*?\/
key INTEGERVALUE = /[+-]?[0-9]+/
key DOUBLEVALUE = /[+-]?[0-9]*"."[0-9]+/
key CHARVALUE = /\'.\'/
key BOOLVALUE = /[true|false]/
key EXPRESSION = //.*?//
key OPDIVIDE = /\//

#############################################################################################################
############################################  Entity definition  ############################################

full entity CodeEntity:
	method file gerateCode()

entity Key inherits CodeEntity:
	property string Name
	property string Expression
	property bool isAssociative
	property string AssociateType
	property bool isContextual
	property bool isIdentity

entity Command inherits CodeEntity

entity CommandBlock inherits CodeEntity:
	property list Command Commands = []
	method add_command(command):
		this.Commands.Add(command)

entity Data inherits CodeEntity

entity ScopeCommand inherits Command:
	property CommandBlock Block

entity Condition inherits CodeEntity

entity Comparation inherits Condition:
	property Data LeftData = null
	property Data RightData = null
	property int ComparationType = 0

entity Relation inherits Condition:
	property Condition LeftCondition = null
	property Condition RightCondition = null
	property int RelationType = 0 

entity If inherits ScopeCommand:
	property Condition Condition

entity ElseIf inherits ScopeCommand:
	property Condition Condition

entity Else inherits ScopeCommand

entity IfStructure inherits Command:
	property If If
	property list ElseIf Elifs
	property Else Else = null

entity RuleBody inherits ScopeCommand:
	property list string Expression = []

entity Rule inherits CodeEntity:
	property string Name
	property list RuleBody Body = []

####################################################################################################################
############################################  Code Geration definition  ############################################

partial entity ElseIf:
	override file gerateCode():
		>> "else if (" this.Condition.gerateCode() ")" newline
		>> "{" newline
		>> tab this.Block.gerateCode()
		>> "}" newline

partial entity If:
	override file gerateCode():
		>> "if (" this.Condition.gerateCode() ")" newline
		>> "{" newline
		>> tab this.Block.gerateCode()
		>> "}" newline

partial entity Else:
	override file gerateCode():
		>> "else" newline
		>> "{" newline
		>> tab this.Block.gerateCode()
		>> "}" newline

partial entity IfStructure:
	override file gerateCode():
		>> if.gerateCode()
		for elif in this.Elifs:
			>> elif.gerateCode()
		if this.else not is null:
			>> else.gerateCode()

partial entity Key:
	override file gerateCode():
		>> "lc.Key" this.Name " = (\"" this.Name ", \"" this.Expression "\");" newline

partial entity RuleBody:
	override file gerateCode():
		for exp in Expression:
			>> exp + " "
		if Code not is null:
			>> Block.gerateCode()

partial entity Rule:
	override file gerateBody():
		>> ": " rb.gerateCode()
		for rb in Body:
			>> "| " rb.gerateCode()
		>> ";"
	override file gerateCode():
		>> this.Name newline
		>> tab + this.gerateBody()

###########################################################################################################
############################################  Rule definition  ############################################

rule processing:
	PROCESSING ALL DOUBLEDOT ENDLINE commandBlock
	PROCESSING LINE DOUBLEDOT ENDLINE commandBlock
	PROCESSING CHARACTER DOUBLEDOT ENDLINE commandBlock

rule basetype:
	INT => "int"
	STRING => "string"
	DECIMAL => "double"
	BOOL => "bool"
	CHAR => "char"
	ANY => "any"

rule type:
	basetype
	TUPLE type
	LIST type
	MAP basetype type
	DOCTREE
	TUPLE
	LIST
	MAP basetype
	ID

rule ruleexpression:
	IDENTITY ruleexpression
	IDENTITY
	identity ruleexpression
	identity

rule ruleelement:
	ruleexpression DOUBLEDOT ENDLINE STARTBLOCK ENDBLOCK
	ruleexpression ENDLINE

rule rulebody:
	ruleelement rulebody
	ruleelement

rule rule:
	RULE identity DOUBLEDOT ENDLINE STARTBLOCK rulebody ENDBLOCK

rule key:
	KEY ID EQUAL EXPRESSION ENDLINE:
		Key key = new Key()
		key.Name = this[1]
		key.Expression = this[3]
		g.add_key(key)
	KEY ID EQUAL EXPRESSION ASSOCIATE basetype ENDLINE:
		Key key = new Key()
		key.Name = this[1]
		key.Expression = this[3]
		key.isAssociative = true
		key.AssociateType = this[5]
		g.add_key(key)
	CONTEXTUAL KEY ID EQUAL EXPRESSION ENDLINE:
		Key key = new Key()
		key.Name = this[2]
		key.Expression = this[4]
		key.isContextual = true
		g.add_key(key)
	CONTEXTUAL KEY ID EQUAL EXPRESSION ASSOCIATE basetype ENDLINE:
		Key key = new Key()
		key.Name = this[2]
		key.Expression = this[4]
		key.isAssociative = true
		key.AssociateType = this[6]
		key.isContextual = true
		g.add_key(key)
	IDENTITY KEY ID EQUAL EXPRESSION ENDLINE:
		Key key = new Key()
		key.Name = this[2]
		key.Expression = this[4]
		key.isIdentity = true
		g.add_key(key)
	IDENTITY KEY ID EQUAL EXPRESSION ASSOCIATE basetype ENDLINE:
		Key key = new Key()
		key.Name = this[2]
		key.Expression = this[4]
		key.isAssociative = true
		key.AssociateType = this[6]
		key.isIdentity = true
		g.add_key(key)

rule startrule:
	START RULE DOUBLEDOT ENDLINE STARTBLOCK rulebody ENDBLOCK

rule reference:
	ID DOT reference
	ID

rule using:
	USING reference ENDLINE

rule include:
	INCLUDE ID

rule element:
	key element
	rule element
	startrule element
	variable element
	function element
	entity element
	file element
	folder element
	compiler element
	using element
	include element
	key
	rule
	startrule
	variable
	function
	entity
	file
	folder
	compiler
	using
	include

rule condition:
	BOOLVALUE
	data IS data
	data NOT IS data
	data IN data
	data SMALLER data
	data SMALLEREQUAL data
	data BIGGER data
	data BIGGEREQUAL data
	condition AND condition
	condition OR condition
	condition XOR condition
	NOT condition
	PARENTHESES condition PARENTHESES_CLOSE

rule data:
	data OPSUM data
	data OPSUBTRACT data
	data OPTIMES data
	data OPDIVIDE data
	data OPPOW data
	data OPMODULE data
	OPSUM data
	OPSUBTRACT data
	INTEGERVALUE
	DOUBLEVALUE
	STRINGVALUE
	CHARVALUE
	EXPRESSION
	NULLVALUE
	condition
	collection
	indexer
	THIS
	BASE
	NEW ID PARENTHESES PARENTHESES_CLOSE
	NEW ID PARENTHESES paramcollection PARENTHESES_CLOSE
	functioncall
	PARENTHESES data PARENTHESES_CLOSE
	data DOT ID
	NEWLINE
	SPACE
	TAB
	ID
	VALUE

rule variable:
	identity
	type identity
	type identity EQUAL data

rule itemcollection:
	data COMMA itemcollection
	data

rule functioncall:
	ID PARENTHESES PARENTHESES_CLOSE
	ID PARENTHESES itemcollection PARENTHESES_CLOSE
	data DOT functioncall

rule listvalue:
	BRACKETS itemcollection BRACKETS_CLOSE
	BRACKETS BRACKETS_CLOSE

rule mapkey:
	data DOUBLEDOT data

rule mapkeycollection:
	mapkey COMMA mapkeycollection
	mapkey

rule mapvalue:
	BRACES mapkeycollection BRACES_CLOSE
	BRACES BRACES_CLOSE

rule tuplevalue:
	PARENTHESES itemcollection PARENTHESES_CLOSE
	PARENTHESES PARENTHESES_CLOSE

rule doctreevalue:
	OPEN data

rule collection:
	listvalue
	mapvalue
	tuplevalue

rule indexer:
	data BRACKETS data BRACKETS_CLOSE

rule paramcollection:
	variable COMMA paramcollection
	variable

rule if:
	IF condition DOUBLEDOT ENDLINE commandBlock

rule elseif:
	ELSE IF condition DOUBLEDOT ENDLINE commandBlock elseif
	ELSE IF condition DOUBLEDOT ENDLINE commandBlock

rule else:
	ELSE DOUBLEDOT ENDLINE commandBlock

rule ifstructure:
	if
	if else
	if elseif
	if elseif else

rule operation:
	OPSUM
	OPSUBTRACT
	OPTIMES
	OPDIVIDE
	OPMODULE
	OPPOW

rule command:
	functioncall ENDLINE
	ifstructure
	processing
	FOR identity IN collection DOUBLEDOT ENDLINE commandBlock
	FOR type identity IN collection DOUBLEDOT ENDLINE commandBlock
	FOR condition DOUBLEDOT ENDLINE commandBlock
	WHILE condition DOUBLEDOT ENDLINE commandBlock
	identity EQUAL data ENDLINE
	identity operation EQUAL data ENDLINE
	RETURN data ENDLINE
	print ENDLINE
	parse ENDLINE
	BREAK ENDLINE
	COMPLETE ENDLINE
	JUMP ENDLINE
	CONTINUE ENDLINE
	SKIP ENDLINE
	NEXT ENDLINE
	variable ENDLINE

rule lambda:
	LAMBDA data
	LAMBDA command

rule commandcollection:
	command
	command commandcollection

rule commandBlock:
	STARTBLOCK commandcollection ENDBLOCK

rule codedefinition:
	ENDLINE commandBlock
	lambda
	ENDLINE STARTBLOCK lambda ENDBLOCK


rule method:
	METHOD ID PARENTHESES PARENTHESES_CLOSE codedefinition
	METHOD ID PARENTHESES paramcollection PARENTHESES_CLOSE codedefinition
	METHOD type ID PARENTHESES PARENTHESES_CLOSE codedefinition
	METHOD type ID PARENTHESES paramcollection PARENTHESES_CLOSE codedefinition
	METHOD ID PARENTHESES PARENTHESES_CLOSE ENDLINE
	METHOD ID PARENTHESES paramcollection PARENTHESES_CLOSE ENDLINE
	METHOD type ID PARENTHESES PARENTHESES_CLOSE ENDLINE
	METHOD type ID PARENTHESES paramcollection PARENTHESES_CLOSE ENDLINE
	OVERRIDE ID PARENTHESES PARENTHESES_CLOSE codedefinition
	OVERRIDE ID PARENTHESES paramcollection PARENTHESES_CLOSE codedefinition
	OVERRIDE type ID PARENTHESES PARENTHESES_CLOSE codedefinition
	OVERRIDE type ID PARENTHESES paramcollection PARENTHESES_CLOSE codedefinition

rule function:
	FUNCTION ID PARENTHESES PARENTHESES_CLOSE codedefinition
	FUNCTION ID PARENTHESES paramcollection PARENTHESES_CLOSE codedefinition
	FUNCTION type ID PARENTHESES PARENTHESES_CLOSE codedefinition
	FUNCTION type ID PARENTHESES paramcollection PARENTHESES_CLOSE codedefinition
	FUNCTION ID PARENTHESES PARENTHESES_CLOSE ENDLINE
	FUNCTION ID PARENTHESES paramcollection PARENTHESES_CLOSE ENDLINE
	FUNCTION type ID PARENTHESES PARENTHESES_CLOSE ENDLINE
	FUNCTION type ID PARENTHESES paramcollection PARENTHESES_CLOSE ENDLINE

rule entity:
	ENTITY ID DOUBLEDOT ENDLINE STARTBLOCK entitybody ENDBLOCK
	ENTITY ID ENDLINE
	ENTITY ID INHERITS ID DOUBLEDOT ENDLINE STARTBLOCK entitybody ENDBLOCK
	ENTITY ID INHERITS ID ENDLINE
	FULL ENTITY ID DOUBLEDOT ENDLINE STARTBLOCK entitybody ENDBLOCK
	FULL ENTITY ID ENDLINE
	FULL ENTITY ID INHERITS ID DOUBLEDOT ENDLINE STARTBLOCK entitybody ENDBLOCK
	FULL ENTITY ID INHERITS ID ENDLINE
	PARTIAL ENTITY ID DOUBLEDOT ENDLINE STARTBLOCK entitybody ENDBLOCK
	PARTIAL ENTITY ID ENDLINE

rule constructor:
	CONSTRUCTOR PARENTHESES PARENTHESES_CLOSE DOUBLEDOT ENDLINE commandBlock
	CONSTRUCTOR PARENTHESES paramcollection PARENTHESES_CLOSE DOUBLEDOT ENDLINE commandBlock
	CONSTRUCTOR PARENTHESES PARENTHESES_CLOSE ENDLINE
	CONSTRUCTOR PARENTHESES_CLOSE itemcollection PARENTHESES_CLOSE ENDLINE 

rule getdefinition:
	GET ENDLINE
	GET codedefinition

rule setdefinition:
	SET ENDLINE
	SET codedefinition

rule getsetdefinition:
	getdefinition
	setdefinition
	getdefinition setdefinition
	setdefinition getdefinition

rule property:
	PROPERTY variable ENDLINE
	PROPERTY READONLY variable ENDLINE
	PROPERTY variable DOUBLEDOT ENDLINE STARTBLOCK getsetdefinition ENDBLOCK

rule entitybody:
	property entitybody
	function entitybody
	constructor entitybody
	variable entitybody
	READONLY variable entitybody
	method entitybody
	property variable
	function
	constructor
	READONLY variable
	method

rule print:
	PRINT text

rule text:
	data text
	data

rule parse:
	PARSE data

rule title:
	TITLE DOUBLEDOT ENDLINE commandBlock
	TITLE lambda ENDLINE
	TITLE

rule content:
	CONTENT DOUBLEDOT ENDLINE commandBlock
	CONTENT lambda ENDLINE
	CONTENT

rule docbody:
	title content
	content title
	title

rule file:
	FILE ID PARENTHESES PARENTHESES_CLOSE DOUBLEDOT ENDLINE STARTBLOCK docbody ENDBLOCK
	FILE ID PARENTHESES paramcollection PARENTHESES_CLOSE DOUBLEDOT ENDLINE STARTBLOCK docbody ENDBLOCK

rule folder:
	FOLDER ID PARENTHESES PARENTHESES_CLOSE DOUBLEDOT ENDLINE STARTBLOCK docbody ENDBLOCK
	FOLDER ID PARENTHESES paramcollection PARENTHESES_CLOSE DOUBLEDOT ENDLINE STARTBLOCK docbody ENDBLOCK

rule compiler:
	COMPILER ID PARENTHESES PARENTHESES_CLOSE DOUBLEDOT ENDLINE commandBlock
	COMPILER ID PARENTHESES paramcollection PARENTHESES_CLOSE DOUBLEDOT ENDLINE commandBlock

start rule:
	element ENDFILE
	ENDFILE

############################################################################################################
############################################  Graph definition  ############################################

entity Graph:
	property list Key keys = []
	method add_key(key):
		keys.Add(key)

Graph g = new Graph()

###########################################################################################################
############################################  File definition  ############################################

file flexfile():
	title => "lex.l"
	content:
		>> "%{" newline
		>> tab "#include <string.h>" newline
		>> tab "#include <stdlib.h>" newline
		>> tab "#include bison.tab.h" newline
		>> "%}" newline
		>> newline "%%" newline newline
		for k in g.keys:
			>> k.gerateCode()
		>> newline "%%" newlin newline
		>> "int yywrap()" newline
		>> "{" newline
		>> tab "return 1;" newline
		>> "}"

file filestructures_cpp():
	title => "FileStructures.cpp"
	content:
		>> "#ifndef FILESTRUCTURES_H" newline
		>> "#define FILESTRUCTURES_H" newline newline
		>> "#include <string>" newline
		>> "using namespace std;" newline newline
		>> "class FileStructures" newline
		>> "{" newline
		>> "public:" newline
		>> "};" newline 
		>> "#endif"

#############################################################################################################
############################################  Folder definition  ############################################

folder src():
	title => "src"
	content
		filestructures_cpp()

#################################################################################################################
############################################  Compilator definition  ############################################

compiler langcompiler(tuple string files):
	for file in files:
		parse open file
	flexfile()
	src()