using Irony.Parsing;

namespace MinCamlSharp.Compiler.Ast
{
	public class Tuple : AstNode
	{
		private AstNode _elements;

		protected override void Initialize(ParseTreeNode parseNode)
		{
			_elements = (AstNode) parseNode.ChildNodes[0].AstNode;
		}
	}
}