using System;
using Irony.Ast;
using Irony.Parsing;
using MinCamlSharp.Compiler.Ast;

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
			var tLParen = ToTerm("(", "LParen");
			var tRParen = ToTerm(")", "RParen");
			var tTrue = ToTerm("true", "True");
			var tFalse = ToTerm("false", "False");
			var tNot = ToTerm("not", "Not");
			var tNumber = new NumberLiteral("Number")
			{
				DefaultFloatType = TypeCode.Single,
				DefaultIntTypes = new[] { TypeCode.Int32 }
			};
			var tMinus = ToTerm("-", "Minus");
			var tPlus = ToTerm("+", "Plus");
			var tMinusDot = ToTerm("-.", "MinusDot");
			var tPlusDot = ToTerm("+.", "PlusDot");
			var tAstDot = ToTerm("*.", "AstDot");
			var tSlashDot = ToTerm("*.", "SlashDot");
			var tEqual = ToTerm("=", "Equal");
			var tLessGreater = ToTerm("<>", "LessGreater");
			var tLessEqual = ToTerm("<=", "LessEqual");
			var tGreaterEqual = ToTerm(">=", "GreaterEqual");
			var tLess = ToTerm("<", "Less");
			var tGreater = ToTerm(">", "Greater");
			var tIf = ToTerm("if", "If");
			var tThen = ToTerm("then", "Then");
			var tElse = ToTerm("else", "Else");
			var tLet = ToTerm("let", "Let");
			var tIn = ToTerm("in", "In");
			var tRec = ToTerm("rec", "Rec");
			var tComma = ToTerm(",", "Comma");
			var tArrayCreate = ToTerm("Array.create", "Array.create");
			var tDot = ToTerm(".", "Dot");
			var tLessMinus = ToTerm("<-", "LessMinus");
			var tSemicolon = ToTerm(";", "Semicolon");
			var tIdentifier = new IdentifierTerminal("Identifier")
			{
				AllFirstChars = "abcdefghijklmnopqrstuvwxyz",
				AllChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890_"
			};

			// Comments
			var blockComment = new CommentTerminal("BlockComment", "(*", "*)");
			NonGrammarTerminals.Add(blockComment);

			// Non-terminals
			var parenthesizedExpression = new NonTerminal("ParenthesizedExpression");
			var emptyParentheses = new NonTerminal("EmptyParentheses");
			var boolean = new NonTerminal("BooleanLiteral");
			var number = new NonTerminal("NumberLiteral");
			var identifier = new NonTerminal("Identifier");
			var get = new NonTerminal("GetExpression");

			var simpleExp = new NonTerminal("SimpleExpression");

			var not = new NonTerminal("Not");
			var unaryOp = new NonTerminal("UnaryOp");
			var unaryOpExpression = new NonTerminal("UnaryOpExpression");
			var binaryOp = new NonTerminal("BinaryOp");
			var binaryOpExpression = new NonTerminal("BinaryOpExpression");
			var ifThenElse = new NonTerminal("If");
			var let = new NonTerminal("Let");
			var letRec = new NonTerminal("LetRec");
			var functionApplication = new NonTerminal("FunctionApplication");
			var letTuple = new NonTerminal("LetTuple");
			var put = new NonTerminal("Put");
			var letUnit = new NonTerminal("LetUnit");
			var arrayCreate = new NonTerminal("ArrayCreate");

			var exp = new NonTerminal("Expression");

			var formalArgs = new NonTerminal("SimpleArgs");
			var functionDefinition = new NonTerminal("FunctionDefinition");
			var actualArgs = new NonTerminal("ActualArgs");
			var tupleElements = new NonTerminal("TupleElements");
			var tuplePattern = new NonTerminal("TuplePattern");

			// Rules

			parenthesizedExpression.Rule = tLParen + exp + tRParen;
			emptyParentheses.Rule = tLParen + tRParen;
			boolean.Rule = tTrue | tFalse;
			number.Rule = tNumber;
			identifier.Rule = tIdentifier;
			get.Rule = simpleExp + tDot + tLParen + exp + tRParen;

			simpleExp.Rule =
				parenthesizedExpression
				| emptyParentheses
				| boolean
				| number
				| identifier
				| get;

			formalArgs.Rule =
				tIdentifier + formalArgs
				| tIdentifier;

			functionDefinition.Rule = tIdentifier + formalArgs + tEqual + exp;

			actualArgs.Rule =
				actualArgs + simpleExp
				| simpleExp;

			tupleElements.Rule =
				tupleElements + tComma + exp
				| exp + tComma + exp;

			tuplePattern.Rule =
				tuplePattern + tComma + tIdentifier
				| tIdentifier + tComma + tIdentifier;

			not.Rule = tNot + exp;

			unaryOp.Rule =
				tMinus
				| tMinusDot;
			unaryOpExpression.Rule = unaryOp + exp;

			binaryOp.Rule =
				tPlus
				| tMinus
				| tEqual
				| tLessGreater
				| tLess
				| tGreater
				| tLessEqual
				| tGreaterEqual
				| tPlusDot
				| tMinusDot
				| tAstDot
				| tSlashDot;
			binaryOpExpression.Rule = exp + binaryOp + exp;

			ifThenElse.Rule = tIf + exp + tThen + exp + tElse + exp;
			let.Rule = tLet + tIdentifier + tEqual + exp + tIn + exp;

			letRec.Rule = tLet + tRec + functionDefinition + tIn + exp;
			letRec.AstConfig.NodeCreator = (c, n) => new LetRec(
				(AstNode) n.ChildNodes[2].AstNode,
				(AstNode) n.ChildNodes[4].AstNode);

			functionApplication.Rule = exp + actualArgs;
			letTuple.Rule = tLet + tLParen + tuplePattern + tRParen + tEqual + exp + tIn + exp;
			put.Rule = simpleExp + tDot + tLParen + exp + tRParen + tLessMinus + exp;
			letUnit.Rule = exp + tSemicolon + exp;
			arrayCreate.Rule = tArrayCreate + simpleExp + simpleExp;

			exp.Rule =
			   simpleExp
			   | not
			   | unaryOpExpression
			   | binaryOpExpression
			   | ifThenElse
			   | let
			   | letRec
			   | functionApplication
			   | tupleElements
			   | letTuple
			   | put
			   | letUnit
			   | arrayCreate;

			Root = exp;

			MarkTransient(exp, simpleExp);
			//LanguageFlags = LanguageFlags.CreateAst;
		}

		private static NonTerminal CreateNonTerminal(string name, AstNodeCreator astNodeCreator)
		{
			return new NonTerminal(name)
			{
				AstConfig = { NodeCreator = astNodeCreator }
			};
		}
	}
}