namespace MinCamlSharp.CodeModel
{
	public abstract class ParseNode
	{
		private ParseNode _parent;

		public ParseNode Parent
		{
			get { return _parent; }
		}

		protected ParseNode GetParentedNode(ParseNode child)
		{
			if (child != null)
				child.SetParent(this);
			return child;
		}

		internal void SetParent(ParseNode node)
		{
			_parent = node;
		}
	}
}