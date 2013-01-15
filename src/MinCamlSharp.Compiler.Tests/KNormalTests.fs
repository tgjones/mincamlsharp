module MinCamlSharp.Compiler.Tests.KNormal

open MinCamlSharp.Compiler
open MinCamlSharp.Compiler.KNormal
open NUnit.Framework
open NUnit.Framework.Constraints

[<Test>]
let CanKNormalize() =
    // Arrange.
    Typing.externalEnvironment := Map.add "print_int" (Type.Fun([Type.Int], Type.Unit)) Map.empty
    let input =
        Syntax.LetRec (
          { name = ("sum", Type.Fun([Type.Int], Type.Int));
            args = [("x", Type.Int)];
            body = Syntax.If(
                       Syntax.LE(Syntax.Var("x"), Syntax.Int(0)),
                       Syntax.Int(0),
                       Syntax.Add(Syntax.App(Syntax.Var("sum"), [Syntax.Sub(Syntax.Var("x"), Syntax.Int(1))]), Syntax.Var("x"))
                     )},
            Syntax.App (
                Syntax.Var("print_int"),
                [Syntax.App(Syntax.Var("sum"), [Syntax.Int(10000)])]
            )
        )

    // Act.
    let output = KNormal.transform input

    // Assert.
    let expectedOutput = 
        LetRec (
          { name = ("sum", Type.Fun([Type.Int], Type.Int));
            args = [("x", Type.Int)];
            body =
                Let(
                    ("Ti3", Type.Int),
                    Int(0),
                    IfLE(
                        "x", "Ti3", Int(0),
                        Let(
                            ("Ti6", Type.Int),
                            Let(
                                ("Ti5", Type.Int),
                                Let(
                                    ("Ti4", Type.Int),
                                    Int(1),
                                    Sub("x", "Ti4")
                                ),
                                App("sum", ["Ti5"])
                            ),
                            Add("Ti6", "x")
                        )
                    )
                )},
            Let(
                ("Ti2", Type.Int),
                Let(
                    ("Ti1", Type.Int),
                    Int(10000),
                    App("sum", ["Ti1"])
                ),
                ExtFunApp("print_int", ["Ti2"])
            )
        )
    Assert.That(output, Is.EqualTo(expectedOutput))