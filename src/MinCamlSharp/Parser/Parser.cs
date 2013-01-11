using MinCamlSharp.CodeModel;
using MinCamlSharp.CodeModel.Tokens;
using MinCamlSharp.Properties;

namespace MinCamlSharp.Parser
{
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

		public CompilationUnitNode Parse()
		{
			return new CompilationUnitNode(new ParseNodeCollection());
            //Eat(TokenType.Eof);
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