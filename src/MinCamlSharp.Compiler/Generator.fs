namespace MinCamlSharp.Compiler

open System
open System.Reflection
open System.Reflection.Emit

type AssemblyGenerator(options : CompilerOptions) =
    member x.GenerateAssembly(tree) =
        let assemblyBuilder =
            AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(options.OutputAssemblyName),
                AssemblyBuilderAccess.Save,
                options.OutputDirectory)

        let moduleBuilder =
            assemblyBuilder.DefineDynamicModule(
                options.OutputAssemblyName,
                options.OutputFileName)

        let mainClassTypeName = options.OutputAssemblyName + ".Program"
        let mainClassFlags = TypeAttributes.Abstract ||| TypeAttributes.Sealed ||| TypeAttributes.Public
        let typeBuilder = moduleBuilder.DefineType(mainClassTypeName, mainClassFlags)

        let mainMethodBuilder = typeBuilder.DefineMethod("Main", MethodAttributes.Public ||| MethodAttributes.Static)
        assemblyBuilder.SetEntryPoint(mainMethodBuilder)

        let mainMethodIlGenerator = mainMethodBuilder.GetILGenerator()
        mainMethodIlGenerator.Emit(OpCodes.Ret)

        typeBuilder.CreateType() |> ignore

        assemblyBuilder.Save(options.OutputFileName)