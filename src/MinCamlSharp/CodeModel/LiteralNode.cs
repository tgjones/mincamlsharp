using MinCamlSharp.CodeModel.Expressions;
using MinCamlSharp.CodeModel.Tokens;

namespace MinCamlSharp.CodeModel
{
	public class LiteralNode : ExpressionNode
	{
		private readonly Token _token;

		public LiteralNode(Token token)
		 {
			 _token = token;
		 }

		public Token Token
		{
			get { return _token; }
		}
	}
}