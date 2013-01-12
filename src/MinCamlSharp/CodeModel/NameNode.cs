using MinCamlSharp.CodeModel.Tokens;

namespace MinCamlSharp.CodeModel
{
	public class NameNode : ParseNode
	{
		private readonly IdentifierToken _identifier;

		public NameNode(IdentifierToken identifier)
		 {
			 _identifier = identifier;
		 }

		public IdentifierToken Identifier
		{
			get { return _identifier; }
		}
	}
}