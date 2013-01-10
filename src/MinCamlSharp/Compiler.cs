using System;
using MinCamlSharp.Generator;

namespace MinCamlSharp
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

			GenerateAssembly();
			if (_hasErrors)
				return false;

			return true;
		}

		private void GenerateAssembly()
		{
			var assemblyGenerator = new AssemblyGenerator(_options);
			assemblyGenerator.GenerateAssembly();
		}
	}
}