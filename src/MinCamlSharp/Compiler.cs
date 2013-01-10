using System;

namespace MinCamlSharp
{
	public class Compiler
	{
		public bool Compile(CompilerOptions options)
		{
			if (options == null)
				throw new ArgumentNullException("options");

			return true;
		}
	}
}