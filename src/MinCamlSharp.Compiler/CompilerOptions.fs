namespace MinCamlSharp.Compiler

open System.IO

type CompilerOptions(sourceFile : string, outputAssemblyName : string, outputFilePath : string) =
    let outputDirectory = Path.GetDirectoryName(outputFilePath)
    let outputFileName = Path.GetFileName(outputFilePath)

    member x.SourceFile = sourceFile
    member x.OutputAssemblyName = outputAssemblyName
    member x.OutputFilePath = outputFilePath
    member x.OutputDirectory = outputDirectory
    member x.OutputFileName = outputFileName