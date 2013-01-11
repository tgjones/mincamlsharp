using MinCamlSharp.CodeModel.Tokens;

namespace MinCamlSharp.CodeModel.Expressions
{
	public class BinaryExpressionNode : ExpressionNode
	{
		private readonly ParseNode _leftChild;
		private readonly TokenType _operatorType;
		private readonly ParseNode _rightChild;

		public BinaryExpressionNode(ParseNode leftChild, TokenType operatorType, ParseNode rightChild)
		{
			_leftChild = GetParentedNode(leftChild);
			_operatorType = operatorType;
			_rightChild = GetParentedNode(rightChild);
		}

		public ParseNode LeftChild
		{
			get { return _leftChild; }
		}

		public TokenType Operator
		{
			get { return _operatorType; }
		}

		public ParseNode RightChild
		{
			get { return _rightChild; }
		}
	}
}