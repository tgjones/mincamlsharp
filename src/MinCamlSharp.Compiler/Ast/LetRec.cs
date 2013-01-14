using Irony.Parsing;

namespace MinCamlSharp.Compiler.Ast
{
	public class LetRec : AstNode
	{
		private AstNode _functionDefinition;
		private AstNode _expression;

		public LetRec(AstNode functionDefinition, AstNode expression)
		{
			_functionDefinition = functionDefinition;
			_expression = expression;
		}

		protected override void Initialize(ParseTreeNode parseNode)
		{
			_functionDefinition = (AstNode) parseNode.ChildNodes[2].AstNode;
			_expression = (AstNode) parseNode.ChildNodes[4].AstNode;
		}
	}
}