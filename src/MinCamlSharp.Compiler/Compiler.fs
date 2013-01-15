namespace MinCamlSharp.Compiler

open System.IO

type Compiler(options : CompilerOptions) =
    let assemblyGenerator = new AssemblyGenerator(options)

//    let limit = ref 1000
//
//    let rec iter n e =
//        Printf.eprintf "iteration %d@." n;
//        if n = 0 then e else
//        let e' = Elim.transform (ConstFold.f (Inline.f (Assoc.f (Beta.f e)))) in
//        if e = e' then e else
//        iter (n - 1) e'
        
    member x.Compile() =
        Lexer.CreateBufferFromString(File.ReadAllText(options.SourceFile))
            |> Parser.exp Lexer.token
            |> Typing.transform
            |> KNormal.transform
            |> Alpha.transform
            |> assemblyGenerator.GenerateAssembly
        true