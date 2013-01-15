module MinCamlSharp.Compiler.Assoc

(* flatten let-bindings (just for prettier printing) *)

open KNormal

let rec transform = function
    | IfEq(x, y, e1, e2) -> IfEq(x, y, transform e1, transform e2)
    | IfLE(x, y, e1, e2) -> IfLE(x, y, transform e1, transform e2)
    | Let(xt, e1, e2) ->
        let rec insert = function
            | Let(yt, e3, e4) -> Let(yt, e3, insert e4)
            | LetRec(fundefs, e) -> LetRec(fundefs, insert e)
            | LetTuple(yts, z, e) -> LetTuple(yts, z, insert e)
            | e -> Let(xt, e, transform e2) in
                insert (transform e1)
    | LetRec({ name = xt; args = yts; body = e1 }, e2) ->
        LetRec({ name = xt; args = yts; body = transform e1 }, transform e2)
    | LetTuple(xts, y, e) -> LetTuple(xts, y, transform e)
    | e -> e