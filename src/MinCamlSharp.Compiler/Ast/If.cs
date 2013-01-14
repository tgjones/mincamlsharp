using Irony.Ast;
using Irony.Parsing;

namespace MinCamlSharp.Compiler.Ast
{
	public class If : AstNode
	{
		private AstNode _condition;
		private AstNode _ifBody;
		private AstNode _elseBody;

		protected override void Initialize(ParseTreeNode parseNode)
		{
			_condition = (AstNode) parseNode.ChildNodes[1].AstNode;
			_ifBody = (AstNode) parseNode.ChildNodes[3].AstNode;
			_elseBody = (AstNode) parseNode.ChildNodes[5].AstNode;
		}
	}
}