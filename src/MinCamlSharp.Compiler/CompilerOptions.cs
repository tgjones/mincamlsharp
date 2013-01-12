using System.IO;

namespace MinCamlSharp
{
	public class CompilerOptions
	{
		public string SourceFile { get; set; }
		public string OutputAssemblyName { get; set; }
		public string OutputFilePath { get; set; }

		public string OutputDirectory
		{
			get { return Path.GetDirectoryName(OutputFilePath); }
		}

		public string OutputFileName
		{
			get { return Path.GetFileName(OutputFilePath); }
		}
	}
}