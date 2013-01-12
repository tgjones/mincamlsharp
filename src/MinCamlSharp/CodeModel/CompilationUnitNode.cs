namespace MinCamlSharp.CodeModel
{
	public class CompilationUnitNode : ParseNode
	{
		private readonly ParseNodeCollection _statements;

		public CompilationUnitNode(ParseNodeCollection statements)
		{
			_statements = statements;
		}

		public ParseNodeCollection Statements
		{
			get { return _statements; }
		}
	}
}