using System.IO;
using NUnit.Framework;

namespace MinCamlSharp.Tests
{
	[TestFixture]
	public class CompilerTests
	{
		[TestFixtureSetUp]
		public void CleanUpOldBinaries()
		{
			if (Directory.Exists("Binaries"))
				Directory.Delete("Binaries");
		}

		private static string[] SourceFiles
		{
			get { return Directory.GetFiles("Assets"); }
		}

		[TestCaseSource("SourceFiles")]
		public void CanCompileAndRunApplication(string sourceFile)
		{
			// Arrange.
			var compiler = new Compiler();
			var options = new CompilerOptions
			{
				SourceFile = sourceFile,
				OutputAssemblyName = "Test",
				OutputFileName = Path.Combine("Binaries", Path.GetFileNameWithoutExtension(sourceFile) + ".exe")
			};

			// Act.
			var result = compiler.Compile(options);

			// Assert.
			Assert.That(result, Is.True);
		}
	}
}