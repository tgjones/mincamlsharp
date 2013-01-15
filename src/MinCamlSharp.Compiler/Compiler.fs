namespace MinCamlSharp.Compiler

open System.IO

type Compiler(options : CompilerOptions) =
    let assemblyGenerator = new AssemblyGenerator(options)
        
    member x.Compile() =
        Lexer.CreateBufferFromString(File.ReadAllText(options.SourceFile))
            |> Parser.exp Lexer.token
            |> Typing.transform
            |> assemblyGenerator.GenerateAssembly
        true