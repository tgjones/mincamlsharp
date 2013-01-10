using System.Diagnostics;
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
			get { return Directory.GetFiles("Assets", "*.ml"); }
		}

		[TestCaseSource("SourceFiles")]
		public void CanCompileApplication(string sourceFile)
		{
			// Arrange / Act.
			var outputFileName = Compile(sourceFile);

			// Assert.
			Assert.That(File.Exists(outputFileName), Is.True);
		}

		[TestCaseSource("SourceFiles")]
		public void CanRunApplication(string sourceFile)
		{
			// Arrange.
			var outputFileName = Compile(sourceFile);
			string referenceFileName = Path.ChangeExtension(sourceFile, ".ref");
			if (File.Exists(referenceFileName))
				Assert.Inconclusive("Could not complete test: no reference file provided.");
			var referenceFileContents = File.ReadAllText(referenceFileName);

			// Act.
			string actualOutput = null;
			var process = new Process
			{
				StartInfo =
				{
					FileName = outputFileName,
					UseShellExecute = false,
					RedirectStandardOutput = true
				}
			};
			process.OutputDataReceived += (sender, args) => actualOutput = args.Data;
			process.Start();
			process.BeginOutputReadLine();
			process.WaitForExit();

			// Assert.
			Assert.That(actualOutput, Is.EqualTo(referenceFileContents));
		}

		private string Compile(string sourceFile)
		{
			// Arrange.
			var compiler = new Compiler();
			string outputFileName = Path.Combine("Binaries", Path.GetFileNameWithoutExtension(sourceFile) + ".exe");
			var options = new CompilerOptions
			{
				SourceFile = sourceFile,
				OutputAssemblyName = "Test",
				OutputFileName = outputFileName
			};

			// Act.
			var result = compiler.Compile(options);

			// Assert.
			Assert.That(result, Is.True);

			return outputFileName;
		}
	}
}