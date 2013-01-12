using System;
using System.IO;
using Irony.Parsing;
using MinCamlSharp.Compiler.Generator;

namespace MinCamlSharp.Compiler
{
	public class Compiler
	{
		private CompilerOptions _options;
		private bool _hasErrors;

		public bool Compile(CompilerOptions options)
		{
			if (options == null)
				throw new ArgumentNullException("options");

			_options = options;

			ParseSource();
			if (_hasErrors)
				return false;

			GenerateAssembly();
			if (_hasErrors)
				return false;

			return true;
		}

		private void ParseSource()
		{
			var grammar = new MinCamlGrammar();
			var parser = new Parser(grammar);
			var parseTree = parser.Parse(File.ReadAllText(_options.SourceFile), _options.SourceFile);
			_hasErrors = parseTree.HasErrors();
		}

		private void GenerateAssembly()
		{
			var assemblyGenerator = new AssemblyGenerator(_options);
			assemblyGenerator.GenerateAssembly();
		}
	}
}