module MinCamlSharp.Compiler.KNormal

type t =
    | Unit
    | Int of int
    | Float of float
    | Neg of Id.t
    | Add of Id.t * Id.t
    | Sub of Id.t * Id.t
    | FNeg of Id.t
    | FAdd of Id.t * Id.t
    | FSub of Id.t * Id.t
    | FMul of Id.t * Id.t
    | FDiv of Id.t * Id.t
    | IfEq of Id.t * Id.t * t * t
    | IfLE of Id.t * Id.t * t * t
    | Let of (Id.t * Type.t) * t * t
    | Var of Id.t
    | LetRec of fundef * t
    | App of Id.t * Id.t list
    | Tuple of Id.t list
    | LetTuple of (Id.t * Type.t) list * Id.t * t
    | Get of Id.t * Id.t
    | Put of Id.t * Id.t * Id.t
    | ExtArray of Id.t
    | ExtFunApp of Id.t * Id.t list
and fundef = { name: Id.t * Type.t; args : (Id.t * Type.t) list; body : t }

let rec fv = function
    | Unit | Int(_) | Float(_) | ExtArray(_) -> Set.empty
    | Neg(x) | FNeg(x) -> Set.singleton x
    | Add(x, y) | Sub(x, y) | FAdd(x, y) | FSub(x, y) | FMul(x, y) | FDiv(x, y) | Get(x, y) ->
        Set.ofList [x; y]
    | IfEq(x, y, e1, e2) | IfLE(x, y, e1, e2) ->
        Set.add x (Set.add y (Set.union (fv e1) (fv e2)))
    | Let((x, t), e1, e2) -> Set.union (fv e1) (Set.remove x (fv e2))
    | Var(x) -> Set.singleton x
    | LetRec({ name = (x, t); args = yts; body = e1 }, e2) ->
        let zs = Set.difference (fv e1) (Set.ofList (List.map fst yts)) in
        Set.difference (Set.union zs (fv e2)) (Set.singleton x)
    | App(x, ys) -> Set.ofList (x :: ys)
    | Tuple(xs) | ExtFunApp(_, xs) -> Set.ofList xs
    | Put(x, y, z) -> Set.ofList [x; y; z]
    | LetTuple(xs, y, e) -> Set.add y (Set.difference (fv e) (Set.ofList (List.map fst xs)))

let insertLet (e, t) k =
    match e with
    | Var(x) -> k x
    | _ ->
        let x = Id.gentmp t in
        let e', t' = k x in
        Let((x, t), e, e'), t'

let rec g env = function
    | Syntax.Unit -> Unit, Type.Unit
    | Syntax.Bool(b) -> Int(if b then 1 else 0), Type.Int
    | Syntax.Int(i) -> Int(i), Type.Int
    | Syntax.Float(d) -> Float(d), Type.Float
    | Syntax.Not(e) -> g env (Syntax.If(e, Syntax.Bool(false), Syntax.Bool(true)))
    | Syntax.Neg(e) ->
        insertLet (g env e)
            (fun x -> Neg(x), Type.Int)
    | Syntax.Add(e1, e2) ->
        insertLet (g env e1)
            (fun x -> insertLet (g env e2) (fun y -> Add(x, y), Type.Int))
    | Syntax.Sub(e1, e2) ->
        insertLet (g env e1)
            (fun x -> insertLet (g env e2) (fun y -> Sub(x, y), Type.Int))
    | Syntax.FNeg(e) ->
        insertLet (g env e)
            (fun x -> FNeg(x), Type.Float)
    | Syntax.FAdd(e1, e2) ->
        insertLet (g env e1)
            (fun x -> insertLet (g env e2) (fun y -> FAdd(x, y), Type.Float))
    | Syntax.FSub(e1, e2) ->
        insertLet (g env e1)
            (fun x -> insertLet (g env e2) (fun y -> FSub(x, y), Type.Float))
    | Syntax.FMul(e1, e2) ->
        insertLet (g env e1)
            (fun x -> insertLet (g env e2) (fun y -> FMul(x, y), Type.Float))
    | Syntax.FDiv(e1, e2) ->
        insertLet (g env e1)
            (fun x -> insertLet (g env e2) (fun y -> FDiv(x, y), Type.Float))
    | Syntax.Eq _ | Syntax.LE _ as cmp ->
        g env (Syntax.If(cmp, Syntax.Bool(true), Syntax.Bool(false)))
    | Syntax.If(Syntax.Not(e1), e2, e3) -> g env (Syntax.If(e1, e3, e2))
    | Syntax.If(Syntax.Eq(e1, e2), e3, e4) ->
        insertLet (g env e1)
            (fun x ->
                insertLet (g env e2)
                    (fun y ->
                        let e3', t3 = g env e3 in
                        let e4', t4 = g env e4 in
                        IfEq(x, y, e3', e4'), t3))
    | Syntax.If(Syntax.LE(e1, e2), e3, e4) ->
        insertLet (g env e1)
            (fun x ->
                insertLet (g env e2)
                    (fun y ->
                        let e3', t3 = g env e3 in
                        let e4', t4 = g env e4 in
                        IfLE(x, y, e3', e4'), t3))
    | Syntax.If(e1, e2, e3) -> g env (Syntax.If(Syntax.Eq(e1, Syntax.Bool(false)), e3, e2))
    | Syntax.Let((x, t), e1, e2) ->
        let e1', t1 = g env e1 in
        let e2', t2 = g (Map.add x t env) e2 in
        Let((x, t), e1', e2'), t2
    | Syntax.Var(x) when Map.containsKey x env -> Var(x), Map.find x env
    | Syntax.Var(x) ->
        (match Map.find x !Typing.externalEnvironment with
        | Type.Array(_) as t -> ExtArray x, t
        | _ -> failwith (Printf.sprintf "external variable %s does not have an array type" x))
    | Syntax.LetRec({ Syntax.name = (x, t); Syntax.args = yts; Syntax.body = e1 }, e2) ->
        let env' = Map.add x t env in
        let e2', t2 = g env' e2 in
        let e1', t1 = g (Map.add_list yts env') e1 in
        LetRec({ name = (x, t); args = yts; body = e1' }, e2'), t2
    | Syntax.App(Syntax.Var(f), e2s) when not (Map.containsKey f env) ->
        (match Map.find f !Typing.externalEnvironment with
        | Type.Fun(_, t) ->
            let rec bind xs = function (* "xs" are identifiers for the arguments *)
                | [] -> ExtFunApp(f, xs), t
                | e2 :: e2s ->
                    insertLet (g env e2)
                        (fun x -> bind (xs @ [x]) e2s) in
            bind [] e2s (* left-to-right evaluation *)
        | _ -> failwith "oops")
    | Syntax.App(e1, e2s) ->
        (match g env e1 with
        | _, Type.Fun(_, t) as g_e1 ->
            insertLet g_e1
                (fun f ->
                    let rec bind xs = function (* "xs" are identifiers for the arguments *)
                        | [] -> App(f, xs), t
                        | e2 :: e2s ->
                            insertLet (g env e2)
                                (fun x -> bind (xs @ [x]) e2s) in
                    bind [] e2s) (* left-to-right evaluation *)
        | _ -> failwith "oops")
    | Syntax.Tuple(es) ->
        let rec bind xs ts = function (* "xs" and "ts" are identifiers and types for the elements *)
            | [] -> Tuple(xs), Type.Tuple(ts)
            | e :: es ->
                let _, t as g_e = g env e in
                insertLet g_e
                    (fun x -> bind (xs @ [x]) (ts @ [t]) es) in
        bind [] [] es
    | Syntax.LetTuple(xts, e1, e2) ->
        insertLet (g env e1)
            (fun y ->
                let e2', t2 = g (Map.add_list xts env) e2 in
                LetTuple(xts, y, e2'), t2)
    | Syntax.Array(e1, e2) ->
        insertLet (g env e1)
            (fun x ->
                let _, t2 as g_e2 = g env e2 in
                insertLet g_e2
                    (fun y ->
                        let l =
                            match t2 with
                            | Type.Float -> "create_float_array"
                            | _ -> "create_array" in
                        ExtFunApp(l, [x; y]), Type.Array(t2)))
    | Syntax.Get(e1, e2) ->
        (match g env e1 with
        | _, Type.Array(t) as g_e1 ->
            insertLet g_e1
                (fun x -> insertLet (g env e2) (fun y -> Get(x, y), t))
        | _ -> failwith "oops")
    | Syntax.Put(e1, e2, e3) ->
        insertLet (g env e1)
            (fun x -> 
                insertLet (g env e2)
                    (fun y -> 
                        insertLet (g env e3)
                            (fun z -> Put(x, y, z), Type.Unit)))

let transform e = fst (g Map.empty e)