module MinCamlSharp.Compiler.ConstFold

open System.Collections.Generic
open KNormal

let memi x env =
    try (match Map.find x env with Int(_) -> true | _ -> false)
    with | :? KeyNotFoundException -> false
let memf x env =
    try (match Map.find x env with Float(_) -> true | _ -> false)
    with | :? KeyNotFoundException -> false
let memt x env =
    try (match Map.find x env with Tuple(_) -> true | _ -> false)
    with | :? KeyNotFoundException -> false

let findi x env = (match Map.find x env with Int(i) -> i | _ -> raise (KeyNotFoundException()))
let findf x env = (match Map.find x env with Float(d) -> d | _ -> raise (KeyNotFoundException()))
let findt x env = (match Map.find x env with Tuple(ys) -> ys | _ -> raise (KeyNotFoundException()))

let rec g env = function
    | Var(x) when memi x env -> Int(findi x env)
    (* | Var(x) when memf x env -> Float(findf x env) *)
    (* | Var(x) when memt x env -> Tuple(findt x env) *)
    | Neg(x) when memi x env -> Int(-(findi x env))
    | Add(x, y) when memi x env && memi y env -> Int(findi x env + findi y env)
    | Sub(x, y) when memi x env && memi y env -> Int(findi x env - findi y env)
    | FNeg(x) when memf x env -> Float(-(findf x env))
    | FAdd(x, y) when memf x env && memf y env -> Float(findf x env + findf y env)
    | FSub(x, y) when memf x env && memf y env -> Float(findf x env - findf y env)
    | FMul(x, y) when memf x env && memf y env -> Float(findf x env * findf y env)
    | FDiv(x, y) when memf x env && memf y env -> Float(findf x env / findf y env)
    | IfEq(x, y, e1, e2) when memi x env && memi y env -> if findi x env = findi y env then g env e1 else g env e2
    | IfEq(x, y, e1, e2) when memf x env && memf y env -> if findf x env = findf y env then g env e1 else g env e2
    | IfEq(x, y, e1, e2) -> IfEq(x, y, g env e1, g env e2)
    | IfLE(x, y, e1, e2) when memi x env && memi y env -> if findi x env <= findi y env then g env e1 else g env e2
    | IfLE(x, y, e1, e2) when memf x env && memf y env -> if findf x env <= findf y env then g env e1 else g env e2
    | IfLE(x, y, e1, e2) -> IfLE(x, y, g env e1, g env e2)
    | Let((x, t), e1, e2) ->
        let e1' = g env e1 in
        let e2' = g (Map.add x e1' env) e2 in
        Let((x, t), e1', e2')
    | LetRec({ name = x; args = ys; body = e1 }, e2) ->
        LetRec({ name = x; args = ys; body = g env e1 }, g env e2)
    | LetTuple(xts, y, e) when memt y env ->
        List.fold2
            (fun e' xt z -> Let(xt, Var(z), e'))
            (g env e)
            xts
            (findt y env)
    | LetTuple(xts, y, e) -> LetTuple(xts, y, g env e)
    | e -> e

let transform = g Map.empty