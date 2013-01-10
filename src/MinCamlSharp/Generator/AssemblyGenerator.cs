using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace MinCamlSharp.Generator
{
	public class AssemblyGenerator
	{
		private readonly CompilerOptions _options;

		public AssemblyGenerator(CompilerOptions options)
		{
			_options = options;
		}

		public void GenerateAssembly()
		{
			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
				new AssemblyName(_options.OutputAssemblyName),
				AssemblyBuilderAccess.Save,
				_options.OutputDirectory);

			var moduleBuilder = assemblyBuilder.DefineDynamicModule(
				_options.OutputAssemblyName, _options.OutputFileName);

			var mainClassTypeName = _options.OutputAssemblyName + ".Program";
			var typeBuilder = moduleBuilder.DefineType(mainClassTypeName, TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.Public);
			var mainMethodBuilder = typeBuilder.DefineMethod("Main", MethodAttributes.Public | MethodAttributes.Static);

			assemblyBuilder.SetEntryPoint(mainMethodBuilder);
			var mainMethodIlGenerator = mainMethodBuilder.GetILGenerator();

			mainMethodIlGenerator.Emit(OpCodes.Ret);

			typeBuilder.CreateType();

			assemblyBuilder.Save(_options.OutputFileName);
		}
	}
}