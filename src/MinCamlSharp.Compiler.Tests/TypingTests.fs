module MinCamlSharp.Compiler.Tests.Typing

open MinCamlSharp.Compiler
open MinCamlSharp.Compiler.Syntax
open NUnit.Framework
open NUnit.Framework.Constraints

[<Test>]
let CanInferTypes() =
    // Arrange.
    let tree =
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

    // Act.
    let result = Typing.transform tree

    // Assert.
    let expectedResult =
        LetRec (
          { name = ("sum", Type.Fun([Type.Int], Type.Int));
            args = [("x", Type.Int)];
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
    Assert.That(result, Is.EqualTo(expectedResult))