using System.Collections.Generic;
using MinCamlSharp.CodeModel;
using MinCamlSharp.CodeModel.Expressions;
using MinCamlSharp.CodeModel.Statements;
using MinCamlSharp.CodeModel.Tokens;
using MinCamlSharp.Properties;

namespace MinCamlSharp.Parser
{
	/// <summary>
	/// MinCaml syntax:
	/// ce ::=							expressions
	/// c								constants
	/// op(e1, ..., en)					primitive operations
	/// if e1 then e2 else e3			conditional branches
	/// let x = e1 in e2				variable definitions
	/// x								variables
	/// let rec x y1 ... yn = e1 in e2	function definitions (mutually recursive)
	/// e e1 ... en						function applications
	/// (e1, ..., en)					tuple creations
	/// let (x1, ..., xn) = e1 in e2	read from tuples
	/// Array.create e1 e2				array creations
	/// e1.(e2)							read from arrays
	/// e1.(e2) &lt;- e3				write to arrays
	/// </summary>
	public class Parser
	{
		public event ErrorEventHandler Error;

		private readonly string _path;
		private readonly Token[] _tokens;
		private BufferPosition _lastErrorPosition;

		protected int TokenIndex { get; set; }

		public Parser(string path, Token[] tokens)
		{
			_path = path;
			_tokens = tokens;
		}

		public ParseNode Parse()
		{
			var result = ParseExpression();
			Eat(TokenType.Eof);
			return result;
		}

		private ParseNode ParseExpression()
		{
			switch (PeekType())
			{
				case TokenType.OpenParen :
				case TokenType.Literal :
					return ParseSimpleExpression();
				case TokenType.If :
					return ParseIf();
				case TokenType.Identifier :
					return ParseIdentifier();
				case TokenType.Let :
					return ParseLet();
				default:
					ReportError(Resources.ParseExpressionExpected);
					return ParseIdentifier();
			}
		}

		private ParseNode ParseSimpleExpression()
		{
			switch (PeekType())
			{
				case TokenType.Literal:
					return new LiteralNode(NextToken());
				case TokenType.OpenParen:
				{
					Eat(TokenType.OpenParen);
					if (PeekType() == TokenType.CloseParen)
					{
						Eat(TokenType.CloseParen);
						return new UnitNode();
					}
					var expression = ParseExpression();
					Eat(TokenType.CloseParen);
					return expression;
				}
				default :
					throw new System.NotImplementedException();
			}
		}

		private DefinitionNode ParseLet()
		{
			Eat(TokenType.Let);
			switch (PeekType())
			{
				case TokenType.Rec :
					return ParseFunctionDefinition();
				case TokenType.OpenParen :
					return ParseLetTuple();
				default :
					return ParseVariableDefinition();
			}
		}

		private FunctionDefinitionNode ParseFunctionDefinition()
		{
			Eat(TokenType.Rec);

			var name = ParseIdentifier();
			var args = ParseFormalArgs();

			Eat(TokenType.Equal);

			var body = ParseExpression();

			return new FunctionDefinitionNode(name, args, body);
		}

		private ParseNodeCollection ParseFormalArgs()
		{
			var args = new ParseNodeCollection();
			while (PeekType() != TokenType.Equal)
				args.Add(ParseIdentifier());
			return args;
		}

		private NameNode ParseIdentifier()
		{
			return new NameNode((IdentifierToken) Eat(TokenType.Identifier));
		}

		private IfElseNode ParseIf()
		{
			Eat(TokenType.If);
			var condition = ParseExpression();
			Eat(TokenType.Then);
			var ifBlock = ParseExpression();
			Eat(TokenType.Else);
			var elseBlock = ParseExpression();

			return new IfElseNode(condition, ifBlock, elseBlock);
		}

		private LetTupleNode ParseLetTuple()
		{
			throw new System.NotImplementedException();
		}

		private VariableDefinitionNode ParseVariableDefinition()
		{
			throw new System.NotImplementedException();
		}

		protected Token Eat(TokenType type)
		{
			if (PeekType() == type)
				return NextToken();
			ReportTokenExpectedError(type);
			return ErrorToken();
		}

		private Token NextToken()
		{
			return _tokens[TokenIndex++];
		}

		protected TokenType PeekType(int index = 0)
		{
			return PeekToken(index).Type;
		}

		private Token PeekToken(int index = 0)
		{
			return _tokens[TokenIndex + index];
		}

		private Token ErrorToken()
		{
			return new Token(TokenType.Error, _path, PeekToken().Position);
		}

		private void ReportTokenExpectedError(TokenType type)
		{
			ReportError(Resources.ParserTokenExpected, Token.GetString(type));
		}

		private void ReportUnexpectedError(TokenType type)
		{
			ReportError(Resources.ParserTokenUnexpected, Token.GetString(type));
		}

		protected void ReportError(string message, params object[] args)
		{
			ReportError(message, PeekToken(), args);
		}

		protected void ReportError(string message, Token token, params object[] args)
		{
			BufferPosition position = token.Position;
			if (Error != null && _lastErrorPosition != position)
				Error(this, new ErrorEventArgs(string.Format(message, args), position));
			_lastErrorPosition = position;
		}
	}
}