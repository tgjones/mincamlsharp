using Irony.Ast;
using Irony.Parsing;

namespace MinCamlSharp.Compiler.Ast
{
	public abstract class AstNode
	{
		public void Init(AstContext context, ParseTreeNode parseNode)
		{
			parseNode.AstNode = this;
		}

		protected abstract void Initialize(ParseTreeNode parseNode);
	}
}