using Irony.Parsing;

namespace MinCamlSharp.Compiler.Ast
{
	public class Put : AstNode
	{
		private AstNode _array;
		private AstNode _index;
		private AstNode _value;

		protected override void Initialize(ParseTreeNode parseNode)
		{
			_array = (AstNode) parseNode.ChildNodes[0].AstNode;
			_index = (AstNode) parseNode.ChildNodes[3].AstNode;
			_value = (AstNode) parseNode.ChildNodes[6].AstNode;
		}
	}
}