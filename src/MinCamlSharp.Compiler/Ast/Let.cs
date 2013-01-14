using Irony.Parsing;

namespace MinCamlSharp.Compiler.Ast
{
	public class Let : AstNode
	{
		private AstNode _identifier;
		private AstNode _value;
		private AstNode _expression;

		protected override void Initialize(ParseTreeNode parseNode)
		{
			_identifier = (AstNode) parseNode.ChildNodes[1].AstNode;
			_value = (AstNode) parseNode.ChildNodes[3].AstNode;
			_expression = (AstNode) parseNode.ChildNodes[5].AstNode;
		}
	}
}