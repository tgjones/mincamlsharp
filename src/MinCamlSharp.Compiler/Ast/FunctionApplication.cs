using Irony.Parsing;

namespace MinCamlSharp.Compiler.Ast
{
	public class FunctionApplication : AstNode
	{
		private AstNode _function;
		private AstNode _args;

		protected override void Initialize(ParseTreeNode parseNode)
		{
			_function = (AstNode) parseNode.ChildNodes[0].AstNode;
			_args = (AstNode) parseNode.ChildNodes[1].AstNode;
		}
	}
}