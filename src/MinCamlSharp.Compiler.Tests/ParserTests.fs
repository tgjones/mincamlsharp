module MinCamlSharp.Compiler.Tests.Parser

open MinCamlSharp.Compiler
open MinCamlSharp.Compiler.Syntax
open NUnit.Framework
open NUnit.Framework.Constraints

[<Test>]
let CanParseSumFunction() =
    // Arrange.
    let code =  @"
        let rec sum x =
            if x <= 0 then 0 else
            sum (x - 1) + x in
        print_int (sum 10000)"
    let lexBuffer = Lexer.CreateBufferFromString(code)

    // Act.
    let tree = Parser.exp Lexer.token lexBuffer

    // Assert.
    let expectedTree =
        LetRec (
          { name = ("sum", Type.Var(ref None));
            args = [("x", Type.Var(ref None))];
            body = If(
                       LE(Var("x"), Int(0)),
                       Int(0),
                       Add(App(Var("sum"), [Sub(Var("x"), Int(1))]), Var("x"))
                     )},
            App (
                Var("print_int"),
                [App(Var("sum"), [Int(10000)])]
            )
        )
    Assert.That(tree, Is.EqualTo(expectedTree))