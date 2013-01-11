using MinCamlSharp.CodeModel;
using MinCamlSharp.CodeModel.Tokens;
using MinCamlSharp.Parser;
using NUnit.Framework;

namespace MinCamlSharp.Tests.Parser
{
	[TestFixture]
	public class LexerTests
	{
		[TestCase("else", TokenType.Else, Category = "Keyword")]
		[TestCase("if", TokenType.If, Category = "Keyword")]
		[TestCase("in", TokenType.In, Category = "Keyword")]
		[TestCase("let", TokenType.Let, Category = "Keyword")]
		[TestCase("rec", TokenType.Rec, Category = "Keyword")]
		[TestCase("then", TokenType.Then, Category = "Keyword")]
		[TestCase("[", TokenType.OpenSquare, Category = "Punctuation")]
		[TestCase("]", TokenType.CloseSquare, Category = "Punctuation")]
		[TestCase("{", TokenType.OpenCurly, Category = "Punctuation")]
		[TestCase("}", TokenType.CloseCurly, Category = "Punctuation")]
		[TestCase("=", TokenType.Equal, Category = "Punctuation")]
		[TestCase(":", TokenType.Colon, Category = "Punctuation")]
		[TestCase(",", TokenType.Comma, Category = "Punctuation")]
		[TestCase("-", TokenType.Minus, Category = "Operators")]
		[TestCase("+", TokenType.Plus, Category = "Operators")]
		[TestCase("<=", TokenType.LessEqual, Category = "Operators")]
		[TestCase("<", TokenType.Less, Category = "Operators")]
		[TestCase(">=", TokenType.GreaterEqual, Category = "Operators")]
		[TestCase(">", TokenType.Greater, Category = "Operators")]
		[TestCase("\"this is a string\"", TokenType.Literal, Category = "String")]
		[TestCase("identifier", TokenType.Identifier, Category = "Identifier")]
		public void CanLexToken(string value, TokenType expectedTokenType)
		{
			// Arrange.
			var lexer = new Lexer(null, value);

			// Act.
			var token = lexer.NextToken();

			// Assert.
			Assert.AreEqual(expectedTokenType, token.Type);
		}

		[Test]
		public void CanLexSumFunction()
		{
			// Arrange.
			var lexer = new Lexer(null, @"let rec sum x =
				if x <= 0 then 0 else
				sum (x - 1) + x in");

			// Act.
			var tokens = lexer.GetTokens();

			// Assert.
			Assert.That(tokens, Has.Length.EqualTo(21));
			Assert.AreEqual(TokenType.Let, tokens[0].Type);
			Assert.AreEqual(TokenType.Rec, tokens[1].Type);
			Assert.AreEqual(TokenType.Identifier, tokens[2].Type);
			Assert.AreEqual("sum", ((IdentifierToken) tokens[2]).Identifier);
			Assert.AreEqual(TokenType.Identifier, tokens[3].Type);
			Assert.AreEqual("x", ((IdentifierToken) tokens[3]).Identifier);
			Assert.AreEqual(TokenType.Equal, tokens[4].Type);
			Assert.AreEqual(TokenType.If, tokens[5].Type);
			Assert.AreEqual(TokenType.Identifier, tokens[6].Type);
			Assert.AreEqual("x", ((IdentifierToken) tokens[6]).Identifier);
			Assert.AreEqual(TokenType.LessEqual, tokens[7].Type);
			Assert.AreEqual(TokenType.Literal, tokens[8].Type);
			Assert.AreEqual(0, ((IntToken) tokens[8]).Value);
			Assert.AreEqual(TokenType.Then, tokens[9].Type);
			Assert.AreEqual(TokenType.Literal, tokens[10].Type);
			Assert.AreEqual(0, ((IntToken) tokens[10]).Value);
			Assert.AreEqual(TokenType.Else, tokens[11].Type);
			Assert.AreEqual(TokenType.Identifier, tokens[12].Type);
			Assert.AreEqual("sum", ((IdentifierToken) tokens[12]).Identifier);
			Assert.AreEqual(TokenType.OpenParen, tokens[13].Type);
			Assert.AreEqual(TokenType.Identifier, tokens[14].Type);
			Assert.AreEqual("x", ((IdentifierToken) tokens[14]).Identifier);
			Assert.AreEqual(TokenType.Minus, tokens[15].Type);
			Assert.AreEqual(TokenType.Literal, tokens[16].Type);
			Assert.AreEqual(1, ((IntToken) tokens[16]).Value);
			Assert.AreEqual(TokenType.CloseParen, tokens[17].Type);
			Assert.AreEqual(TokenType.Plus, tokens[18].Type);
			Assert.AreEqual(TokenType.Identifier, tokens[19].Type);
			Assert.AreEqual("x", ((IdentifierToken) tokens[19]).Identifier);
			Assert.AreEqual(TokenType.In, tokens[20].Type);
		}

		[Test]
		public void CanLexComment()
		{
			// Arrange.
			var lexer = new Lexer(null, "(* foo *)");

			// Act.
			var tokens = lexer.GetTokens();

			// Assert.
			Assert.That(tokens, Is.Empty);
		}

		[Test]
		public void BadlyFormattedCommentResultsInErrorToken()
		{
			// Arrange.
			var lexer = new Lexer(null, "(* this is a bad comment");

			// Act.
			var tokens = lexer.GetTokens();

			// Assert.
			Assert.AreEqual(TokenType.Error, tokens[0].Type);
		}
	}
}