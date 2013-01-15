module MinCamlSharp.Compiler.Typing

open System
open Syntax

exception Unify of Type.t * Type.t
exception Error of t * Type.t * Type.t

let externalEnvironment = ref Map.empty<Id.t, Type.t>

let rec derefType = function
    | Type.Fun(t1s, t2) -> Type.Fun(List.map derefType t1s, derefType t2)
    | Type.Tuple(ts) -> Type.Tuple(List.map derefType ts)
    | Type.Array(t) -> Type.Array(derefType t)
    | Type.Var({ contents = None } as r) ->
        Printf.eprintf "uninstantiated type variable detected; assuming int@"
        r := Some(Type.Int);
        Type.Int
    | Type.Var({ contents = Some(t) } as r) ->
        let t' = derefType t in
        r := Some(t');
        t'
    | t -> t

let rec derefIdType (x, t) = (x, derefType t)

let rec derefTerm = function
    | Not(e) -> Not(derefTerm e)
    | Neg(e) -> Neg(derefTerm e)
    | Add(e1, e2) -> Add(derefTerm e1, derefTerm e2)
    | Sub(e1, e2) -> Sub(derefTerm e1, derefTerm e2)
    | Eq(e1, e2) -> Eq(derefTerm e1, derefTerm e2)
    | LE(e1, e2) -> LE(derefTerm e1, derefTerm e2)
    | FNeg(e) -> FNeg(derefTerm e)
    | FAdd(e1, e2) -> FAdd(derefTerm e1, derefTerm e2)
    | FSub(e1, e2) -> FSub(derefTerm e1, derefTerm e2)
    | FMul(e1, e2) -> FMul(derefTerm e1, derefTerm e2)
    | FDiv(e1, e2) -> FDiv(derefTerm e1, derefTerm e2)
    | If(e1, e2, e3) -> If(derefTerm e1, derefTerm e2, derefTerm e3)
    | Let(xt, e1, e2) -> Let(derefIdType xt, derefTerm e1, derefTerm e2)
    | LetRec({ name = xt; args = yts; body = e1 }, e2) ->
        LetRec({ name = derefIdType xt;
                 args = List.map derefIdType yts;
                 body = derefTerm e1 },
               derefTerm e2)
    | App(e, es) -> App(derefTerm e, List.map derefTerm es)
    | Tuple(es) -> Tuple(List.map derefTerm es)
    | LetTuple(xts, e1, e2) -> LetTuple(List.map derefIdType xts, derefTerm e1, derefTerm e2)
    | Array(e1, e2) -> Array(derefTerm e1, derefTerm e2)
    | Get(e1, e2) -> Get(derefTerm e1, derefTerm e2)
    | Put(e1, e2, e3) -> Put(derefTerm e1, derefTerm e2, derefTerm e3)
    | e -> e

let rec occur r1 = function
    | Type.Fun(t2s, t2) -> List.exists (occur r1) t2s || occur r1 t2
    | Type.Tuple(t2s) -> List.exists (occur r1) t2s
    | Type.Array(t2) -> occur r1 t2
    | Type.Var(r2) when !r1 <> None && !r2 <> None && !r1 = !r2 -> true
    | Type.Var({ contents = None }) -> false
    | Type.Var({ contents = Some(t2) }) -> occur r1 t2
    | _ -> false

let rec unify t1 t2 =
    match t1, t2 with
    | Type.Unit, Type.Unit | Type.Bool, Type.Bool | Type.Int, Type.Int | Type.Float, Type.Float -> ()
    | Type.Fun(t1s, t1'), Type.Fun(t2s, t2') ->
        try List.iter2 unify t1s t2s
        with :? ArgumentException -> raise (Unify(t1, t2));
        unify t1' t2'
    | Type.Tuple(t1s), Type.Tuple(t2s) ->
        try List.iter2 unify t1s t2s
        with :? ArgumentException -> raise (Unify(t1, t2));
    | Type.Array(t1), Type.Array(t2) -> unify t1 t2
    | Type.Var(r1), Type.Var(r2) when r1 = r2 -> ()
    | Type.Var({ contents = Some(t1') }), _ -> unify t1' t2
    | _, Type.Var({ contents = Some(t2') }) -> unify t1 t2'
    | Type.Var({ contents = None } as r1), _ ->
        if occur r1 t2 then raise (Unify(t1, t2));
        r1 := Some(t2)
    | _, Type.Var({ contents = None } as r2) ->
        if occur r2 t1 then raise (Unify(t1, t2));
        r2 := Some(t1)
    | _, _ -> raise (Unify(t1, t2))

let rec g env e =
    try
        match e with
        | Unit -> Type.Unit
        | Bool(_) -> Type.Bool
        | Int(_) -> Type.Int
        | Float(_) -> Type.Float
        | Not(e) ->
            unify Type.Bool (g env e);
            Type.Bool
        | Neg(e) ->
            unify Type.Int (g env e);
            Type.Int
        | Add(e1, e2) | Sub(e1, e2) ->
            unify Type.Int (g env e1);
            unify Type.Int (g env e2);
            Type.Int
        | FNeg(e) ->
            unify Type.Float (g env e);
            Type.Float
        | FAdd(e1, e2) | FSub(e1, e2) | FMul(e1, e2) | FDiv(e1, e2) ->
            unify Type.Float (g env e1);
            unify Type.Float (g env e2);
            Type.Float
        | Eq(e1, e2) | LE(e1, e2) ->
            unify (g env e1) (g env e2);
            Type.Bool
        | If(e1, e2, e3) ->
            unify (g env e1) Type.Bool
            let t2 = g env e2 in
            let t3 = g env e3 in
            unify t2 t3;
            t2
        | Let((x, t), e1, e2) ->
            unify t (g env e1);
            g (Map.add x t env) e2
        | Var(x) when Map.containsKey x env -> Map.find x env
        | Var(x) when Map.containsKey x !externalEnvironment -> Map.find x !externalEnvironment
        | Var(x) ->
            Printf.eprintf "free variable %s assumed as external@." x;
            let t = Type.gentyp () in
            externalEnvironment := Map.add x t !externalEnvironment;
            t
        | LetRec({ name = (x, t); args = yts; body = e1 }, e2) ->
            let env = Map.add x t env in
            unify t (Type.Fun(List.map snd yts, g (Map.add_list yts env) e1));
            g env e2
        | App(e, es) ->
            let t = Type.gentyp () in
            unify (g env e) (Type.Fun(List.map (g env) es, t));
            t
        | Tuple(es) -> Type.Tuple(List.map (g env) es)
        | LetTuple(xts, e1, e2) ->
            unify (Type.Tuple(List.map snd xts)) (g env e1);
            g (Map.add_list xts env) e2
        | Array(e1, e2) ->
            unify (g env e1) Type.Int;
            Type.Array(g env e2)
        | Get(e1, e2) ->
            let t = Type.gentyp () in
            unify (Type.Array(t)) (g env e1);
            unify Type.Int (g env e2);
            t
        | Put(e1, e2, e3) ->
            let t = g env e3 in
            unify (Type.Array(t)) (g env e1);
            unify Type.Int (g env e2);
            Type.Unit
    with Unify(t1, t2) -> raise (Error(derefTerm e, derefType t1, derefType t2))

let transform e =  
    externalEnvironment := Map.empty
    
    try unify Type.Unit (g Map.empty e)
    with Unify _ -> failwith "top level does not have type unit";

    externalEnvironment := Map.mapRange derefType !externalEnvironment;
    derefTerm e