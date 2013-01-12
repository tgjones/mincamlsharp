using MinCamlSharp.CodeModel.Expressions;

namespace MinCamlSharp.CodeModel.Statements
{
	public class FunctionDefinitionNode : DefinitionNode
	{
		private readonly NameNode _name;
		private readonly ParseNodeCollection _args;
		private readonly ParseNode _body;

		public FunctionDefinitionNode(NameNode name, ParseNodeCollection args, ParseNode body)
		{
			_name = name;
			_args = args;
			_body = body;
		}

		public NameNode Name
		{
			get { return _name; }
		}

		public ParseNodeCollection Args
		{
			get { return _args; }
		}

		public ParseNode Body
		{
			get { return _body; }
		}
	}
}