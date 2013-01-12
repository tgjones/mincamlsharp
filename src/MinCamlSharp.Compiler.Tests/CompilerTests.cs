using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace MinCamlSharp.Compiler.Tests
{
	[TestFixture]
	public class CompilerTests
	{
		[TestFixtureSetUp]
		public void PrepareBinariesDirectory()
		{
			if (Directory.Exists("Binaries"))
				Directory.Delete("Binaries", true);
			Directory.CreateDirectory("Binaries");
		}

		private static string[] SourceFiles
		{
			get { return Directory.GetFiles("Sources", "*.ml"); }
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
		[Ignore("Not ready for this yet")]
		public void CanRunApplication(string sourceFile)
		{
			// Arrange.
			var outputFilePath = Compile(sourceFile);
			string referenceFileName = Path.ChangeExtension(sourceFile, ".txt");
			if (File.Exists(referenceFileName))
				Assert.Fail("Could not complete test: no reference file provided.");
			var referenceFileContents = File.ReadAllText(referenceFileName);

			// Act.
			string actualOutput = null;
			var process = new Process
			{
				StartInfo =
				{
					FileName = outputFilePath,
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
			string outputFilePath = Path.Combine("Binaries", Path.GetFileNameWithoutExtension(sourceFile) + ".exe");
			var options = new CompilerOptions
			{
				SourceFile = sourceFile,
				OutputAssemblyName = "Test",
				OutputFilePath = outputFilePath
			};

			// Act.
			var result = compiler.Compile(options);

			// Assert.
			Assert.That(result, Is.True);

			return outputFilePath;
		}
	}
}