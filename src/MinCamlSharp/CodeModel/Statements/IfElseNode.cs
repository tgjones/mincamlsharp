namespace MinCamlSharp.CodeModel.Statements
{
	public class IfElseNode : StatementNode
	{
		private readonly ParseNode _condition;
		private readonly ParseNode _ifBlock;
		private readonly ParseNode _elseBlock;

		public IfElseNode(ParseNode condition, ParseNode ifBlock, ParseNode elseBlock)
		{
			_condition = GetParentedNode(condition);
			_ifBlock = GetParentedNode(ifBlock);
			_elseBlock = GetParentedNode(elseBlock);
		}

		public ParseNode Condition
		{
			get { return _condition; }
		}

		public ParseNode IfBlock
		{
			get { return _ifBlock; }
		}

		public ParseNode ElseBlock
		{
			get { return _elseBlock; }
		}
	}
}