namespace MinCamlSharp.CodeModel
{
	public class CompilationUnitNode : ParseNode
	{
		private readonly ParseNodeCollection _members;

		public CompilationUnitNode(ParseNodeCollection members)
		{
			_members = members;
		}

		public ParseNodeCollection Members
		{
			get { return _members; }
		}
	}
}