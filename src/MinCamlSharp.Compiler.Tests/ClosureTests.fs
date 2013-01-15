module MinCamlSharp.Compiler.Tests.Closure

open MinCamlSharp.Compiler
open MinCamlSharp.Compiler.Closure
open NUnit.Framework
open NUnit.Framework.Constraints

[<Test>]
let TestClosureConversion() =
    // Arrange.
    let input = 
        KNormal.LetRec (
          { name = ("sum.1", Type.Fun([Type.Int], Type.Int));
            args = [("x.2", Type.Int)];
            body =
                KNormal.Let(
                    ("Ti3.3", Type.Int),
                    KNormal.Int(0),
                    KNormal.IfLE(
                        "x.2", "Ti3.3", KNormal.Int(0),
                        KNormal.Let(
                            ("Ti6.4", Type.Int),
                            KNormal.Let(
                                ("Ti5.5", Type.Int),
                                KNormal.Let(
                                    ("Ti4.6", Type.Int),
                                    KNormal.Int(1),
                                    KNormal.Sub("x.2", "Ti4.6")
                                ),
                                KNormal.App("sum.1", ["Ti5.5"])
                            ),
                            KNormal.Add("Ti6.4", "x.2")
                        )
                    )
                )},
            KNormal.Let(
                ("Ti2.7", Type.Int),
                KNormal.Let(
                    ("Ti1.8", Type.Int),
                    KNormal.Int(10000),
                    KNormal.App("sum.1", ["Ti1.8"])
                ),
                KNormal.ExtFunApp("print_int", ["Ti2.7"])
            )
        )

    // Act.
    let output = Closure.transform input

    // Assert.
    let expectedOutput =
        Prog (
          [{ name = (Id.L("sum.1"), Type.Fun([Type.Int], Type.Int));
            args = [("x.2", Type.Int)];
            formal_fv = [];
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
                                AppDir(Id.L("sum.1"), ["Ti5.5"])
                            ),
                            Add("Ti6.4", "x.2")
                        )
                    )
                )}],
            Let(
                ("Ti2.7", Type.Int),
                Let(
                    ("Ti1.8", Type.Int),
                    Int(10000),
                    AppDir(Id.L("sum.1"), ["Ti1.8"])
                ),
                AppDir(Id.L("min_caml_print_int"), ["Ti2.7"])
            )
        )
    Assert.That(output, Is.EqualTo(expectedOutput))