module MinCamlSharp.Compiler.Tests.Alpha

open MinCamlSharp.Compiler
open MinCamlSharp.Compiler.KNormal
open NUnit.Framework
open NUnit.Framework.Constraints

[<Test>]
let CanAlphaNormalize() =
    // Arrange.
    let input = 
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

    // Act.
    let output = Alpha.transform input

    // Assert.
    let expectedOutput = 
        LetRec (
          { name = ("sum.1", Type.Fun([Type.Int], Type.Int));
            args = [("x.2", Type.Int)];
            body =
                Let(
                    ("Ti3.3", Type.Int),
                    Int(0),
                    IfLE(
                        "x.2", "Ti3.3", Int(0),
                        Let(
                            ("Ti6.4", Type.Int),
                            Let(
                                ("Ti5.5", Type.Int),
                                Let(
                                    ("Ti4.6", Type.Int),
                                    Int(1),
                                    Sub("x.2", "Ti4.6")
                                ),
                                App("sum.1", ["Ti5.5"])
                            ),
                            Add("Ti6.4", "x.2")
                        )
                    )
                )},
            Let(
                ("Ti2.7", Type.Int),
                Let(
                    ("Ti1.8", Type.Int),
                    Int(10000),
                    App("sum.1", ["Ti1.8"])
                ),
                ExtFunApp("print_int", ["Ti2.7"])
            )
        )
    Assert.That(output, Is.EqualTo(expectedOutput))