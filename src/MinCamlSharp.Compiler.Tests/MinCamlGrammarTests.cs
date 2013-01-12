using Irony.Parsing;
using NUnit.Framework;

namespace MinCamlSharp.Compiler.Tests
{
	[TestFixture]
	public class MinCamlGrammarTests
	{
		[Test]
		public void CanParseSumFunction()
		{
			// Arrange.
			var grammar = new MinCamlGrammar();
			var parser = new Parser(grammar);

			// Act.
			var parseTree = parser.Parse(@"
				let rec sum x =
					if x <= 0 then 0 else
					sum (x - 1) + x in
				print_int (sum 10000)");
			
			// Assert.
			Assert.That(parseTree, Is.Not.Null);
			Assert.That(parseTree.HasErrors(), Is.False);
		}
	}
}