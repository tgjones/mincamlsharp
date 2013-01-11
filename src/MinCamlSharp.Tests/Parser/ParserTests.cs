using MinCamlSharp.CodeModel;
using MinCamlSharp.CodeModel.Tokens;
using MinCamlSharp.Parser;
using NUnit.Framework;

namespace MinCamlSharp.Tests.Parser
{
	[TestFixture]
	public class ParserTests
	{
		[Test]
		public void CanParseSumFunction()
		{
			// Arrange.
			var lexer = new Lexer(null, @"let rec sum x =
				if x <= 0 then 0 else
				sum (x - 1) + x in");
			var parser = new MinCamlSharp.Parser.Parser(null, lexer.GetTokens());

			// Act.
			var compilationUnitNode = parser.Parse();

			// Assert.
			//Assert.AreEqual("basic_material", fragment.Name);
			//Assert.IsNotNull(fragment.Parameters);
			//Assert.AreEqual(2, fragment.Parameters.VariableDeclarations.Count);
			//Assert.AreEqual(TokenType.Float, fragment.Parameters.VariableDeclarations[0].DataType);
			//Assert.AreEqual("alpha", fragment.Parameters.VariableDeclarations[0].Name);
			//Assert.AreEqual("ALPHA", fragment.Parameters.VariableDeclarations[0].Semantic);
			//Assert.AreEqual(TokenType.Float3, fragment.Parameters.VariableDeclarations[1].DataType);
			//Assert.AreEqual("color", fragment.Parameters.VariableDeclarations[1].Name);
		}
	}
}