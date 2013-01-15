module MinCamlSharp.Compiler.Elim

open KNormal

let rec effect = function
    | Let(_, e1, e2) | IfEq(_, _, e1, e2) | IfLE(_, _, e1, e2) -> effect e1 || effect e2
    | LetRec(_, e) | LetTuple(_, _, e) -> effect e
    | App _ | Put _ | ExtFunApp _ -> true
    | _ -> false

let rec transform = function
    | IfEq(x, y, e1, e2) -> IfEq(x, y, transform e1, transform e2)
    | IfLE(x, y, e1, e2) -> IfLE(x, y, transform e1, transform e2)
    | Let((x, t), e1, e2) ->
        let e1' = transform e1 in
        let e2' = transform e2 in
        if effect e1' || Set.contains x (fv e2') then Let((x, t), e1', e2') else
        (Printf.eprintf "eliminating variable %s@." x;
         e2')
    | LetRec({ name = (x, t); args = yts; body = e1 }, e2) ->
        let e2' = transform e2 in
        if Set.contains x (fv e2') then
            LetRec({ name = (x, t); args = yts; body = transform e1 }, e2')
        else
            (Printf.eprintf "eliminating function %s@." x;
             e2')
    | LetTuple(xts, y, e) ->
        let xs = List.map fst xts in
        let e' = transform e in
        let live = fv e' in
        if List.exists (fun x -> Set.contains x live) xs then LetTuple(xts, y, e') else
            (Printf.eprintf "eliminating variables %s@." (Id.pp_list xs);
             e')
    | e -> e