using System;
using Irony.Parsing;

namespace MinCamlSharp.Compiler
{
	/// <summary>
	/// Based on https://github.com/steshaw/mincaml/blob/master/parser.mly
	/// </summary>
	[Language("MinCaml", "0.1", "Minimal subset of Objective Caml")]
	public class MinCamlGrammar : Grammar
	{
		 public MinCamlGrammar()
		 {
			 // Terminals
			 var LPAREN = ToTerm("(", "LParen");
			 var RPAREN = ToTerm(")", "RParen");
			 var TRUE = ToTerm("true", "True");
			 var FALSE = ToTerm("false", "False");
			 var NOT = ToTerm("not", "Not");
			 var NUMBER = new NumberLiteral("Number")
			 {
				 DefaultFloatType = TypeCode.Single,
				 DefaultIntTypes = new[] { TypeCode.Int32 }
			 };
			 var MINUS = ToTerm("-", "Minus");
			 var PLUS = ToTerm("+", "Plus");
			 var MINUS_DOT = ToTerm("-.", "MinusDot");
			 var PLUS_DOT = ToTerm("+.", "PlusDot");
			 var AST_DOT = ToTerm("*.", "AstDot");
			 var SLASH_DOT = ToTerm("*.", "SlashDot");
			 var EQUAL = ToTerm("=", "Equal");
			 var LESS_GREATER = ToTerm("<>", "LessGreater");
			 var LESS_EQUAL = ToTerm("<=", "LessEqual");
			 var GREATER_EQUAL = ToTerm(">=", "GreaterEqual");
			 var LESS = ToTerm("<", "Less");
			 var GREATER = ToTerm(">", "Greater");
			 var IF = ToTerm("if", "If");
			 var THEN = ToTerm("then", "Then");
			 var ELSE = ToTerm("else", "Else");
			 var LET = ToTerm("let", "Let");
			 var IN = ToTerm("in", "In");
			 var REC = ToTerm("rec", "Rec");
			 var COMMA = ToTerm(",", "Comma");
			 var UNDERSCORE = ToTerm("_", "Underscore");
			 var ARRAY_CREATE = ToTerm("Array.create", "Array.create");
			 var DOT = ToTerm(".", "Dot");
			 var LESS_MINUS = ToTerm("<-", "LessMinus");
			 var SEMICOLON = ToTerm(";", "Semicolon");
			 var IDENT = new IdentifierTerminal("Identifier")
			 {
				 AllFirstChars = "abcdefghijklmnopqrstuvwxyz",
				 AllChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890_"
			 };

			 var BlockComment = new CommentTerminal("BlockComment", "(*", "*)");
			 NonGrammarTerminals.Add(BlockComment);

			 // Non-terminals
			 var boolean_literal = new NonTerminal("boolean_literal");
			 boolean_literal.Rule = TRUE | FALSE;

			 var exp = new NonTerminal("exp");
			
			 var simpleExp = new NonTerminal("simple_exp");
			 simpleExp.Rule = 
				 LPAREN + exp + RPAREN
				 | LPAREN + RPAREN
				 | boolean_literal
				 | NUMBER
				 | IDENT
				 | simpleExp + DOT + LPAREN + exp + RPAREN;

			 var formalArgs = new NonTerminal("formal_args");
			 formalArgs.Rule = 
				 IDENT + formalArgs
				 | IDENT;

			 var fundef = new NonTerminal("fundef");
			 fundef.Rule = IDENT + formalArgs + EQUAL + exp;

			 var actualArgs = new NonTerminal("actual_args");
			 actualArgs.Rule = 
				 actualArgs + simpleExp
				 | simpleExp;

			 var elems = new NonTerminal("elems");
			 elems.Rule =
				 elems + COMMA + exp
				 | exp + COMMA + exp;

			 var pat = new NonTerminal("pat");
			 pat.Rule = 
				 pat + COMMA + IDENT
				 | IDENT + COMMA + IDENT;

			 exp.Rule =
				simpleExp
				| NOT + exp
				| MINUS + exp
				| exp + PLUS + exp
				| exp + MINUS + exp
				| exp + EQUAL + exp
				| exp + LESS_GREATER + exp
				| exp + LESS + exp
				| exp + GREATER + exp
				| exp + LESS_EQUAL + exp
				| exp + GREATER_EQUAL + exp
				| IF + exp + THEN + exp + ELSE + exp
				| MINUS_DOT + exp
				| exp + PLUS_DOT + exp
				| exp + MINUS_DOT + exp
				| exp + AST_DOT + exp
				| exp + SLASH_DOT + exp
				| LET + IDENT + EQUAL + exp + IN + exp
				| LET + REC + fundef + IN + exp
				| exp + actualArgs
				| elems
				| LET + LPAREN + pat + RPAREN + EQUAL + exp + IN + exp
				| simpleExp + DOT + LPAREN + exp + RPAREN + LESS_MINUS + exp
				| exp + SEMICOLON + exp
				| ARRAY_CREATE + simpleExp + simpleExp;

			 Root = exp;
		 }
	}
}