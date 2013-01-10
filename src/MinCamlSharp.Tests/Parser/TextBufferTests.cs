using MinCamlSharp.Parser;
using NUnit.Framework;

namespace MinCamlSharp.Tests.Parser
{
	[TestFixture]
	public class TextBufferTests
	{
		[Test]
		public void CanPeekCharFirst()
		{
			// Arrange.
			var buffer = new TextBuffer("This is some text");

			// Act.
			char c = buffer.PeekChar();

			// Assert.
			Assert.AreEqual('T', c);
		}

		[Test]
		public void CanPeekCharWithIndex()
		{
			// Arrange.
			var buffer = new TextBuffer("This is some text");

			// Act.
			char c = buffer.PeekChar(10);

			// Assert.
			Assert.AreEqual('m', c);
		}

		[Test]
		public void CanGetNextChar()
		{
			// Arrange.
			var buffer = new TextBuffer("This is some text");

			// Act.
			char c1 = buffer.NextChar();
			char c2 = buffer.NextChar();

			// Assert.
			Assert.AreEqual('T', c1);
			Assert.AreEqual('h', c2);
		}

		[Test]
		public void PeekCharDetectsInvalidPosition()
		{
			// Arrange.
			var buffer = new TextBuffer("This is some text");

			// Act.
			char c = buffer.PeekChar(20);

			// Assert.
			Assert.AreEqual('\0', c);
		}

		[Test]
		public void CorrectlyReportsColumnOnSingleLine()
		{
			// Arrange.
			var buffer = new TextBuffer(string.Format("Abc{0}Defg{0}C", "\r\n"));

			// Act.
			buffer.NextChar();
			buffer.NextChar();

			// Assert.
			Assert.AreEqual(2, buffer.Column);
		}

		[Test]
		public void CorrectlyReportsLine()
		{
			// Arrange.
			var buffer = new TextBuffer(string.Format("Abc{0}Defg{0}C", "\r\n"));

			// Act.
			buffer.NextChar();
			buffer.NextChar();
			buffer.NextChar();
			buffer.NextChar();
			buffer.NextChar();

			// Assert.
			Assert.AreEqual(1, buffer.Column);
			Assert.AreEqual(1, buffer.Line);
		}

		[Test]
		public void CorrectlyReportsRemainingLength()
		{
			// Arrange.
			var buffer = new TextBuffer("This is some text");

			// Act.
			buffer.NextChar();
			buffer.NextChar();
			buffer.NextChar();
			buffer.NextChar();
			buffer.NextChar();

			// Assert.
			Assert.AreEqual(12, buffer.RemainingLength);
		}

		[Test]
		public void CorrectlyReportsIsEofWhenFalse()
		{
			// Arrange.
			var buffer = new TextBuffer("Abc");

			// Act.
			buffer.NextChar();

			// Assert.
			Assert.AreEqual(false, buffer.IsEof);
		}

		[Test]
		public void CorrectlyReportsIsEofWhenTrue()
		{
			// Arrange.
			var buffer = new TextBuffer("Abc");

			// Act.
			buffer.NextChar();
			buffer.NextChar();
			buffer.NextChar();

			// Assert.
			Assert.AreEqual(true, buffer.IsEof);
		}

		[Test]
		public void CorrectlyReportsPosition()
		{
			// Arrange.
			var buffer = new TextBuffer(string.Format("Abc{0}Defg{0}C", "\r\n"));

			// Act.
			buffer.NextChar();
			buffer.NextChar();
			buffer.NextChar();
			buffer.NextChar();
			buffer.NextChar();
			buffer.NextChar();

			BufferPosition position = buffer.Position;

			// Assert.
			Assert.AreEqual(2, position.Column);
			Assert.AreEqual(1, position.Line);
			Assert.AreEqual(7, position.Offset);
		}
	}
}