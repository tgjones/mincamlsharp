using System.ComponentModel;

namespace MinCamlSharp.CodeModel.Tokens
{
	public enum TokenType
	{
		[Description("else")]
		Else,

		[Description("if")]
		If,

		[Description("in")]
		In,

		[Description("let")]
		Let,

		[Description("rec")]
		Rec,

		[Description("then")]
		Then,

		[Description("Identifier")] // Everything before this is a keyword.
		Identifier,

		[Description("EOF")]
		Eof,

		[Description("[")]
		OpenSquare,

		[Description("]")]
		CloseSquare,

		[Description("{")]
		OpenCurly,

		[Description("}")]
		CloseCurly,

		[Description("(")]
		OpenParen,

		[Description(")")]
		CloseParen,

		[Description("-")]
		Minus,

		[Description("+")]
		Plus,

		[Description("<=")]
		LessEqual,

		[Description("<")]
		Less,

		[Description(">=")]
		GreaterEqual,

		[Description(">")]
		Greater,

		[Description("=")]
		Equal,

		[Description(",")]
		Comma,

		[Description(":")]
		Colon,

		[Description("Error")]
		Error,

		[Description(";")]
		Semicolon,

		//[Description("Indent")]
		//Indent,

		//[Description("Dedent")]
		//Dedent,

		[Description("Literal")]
		Literal
	}
}