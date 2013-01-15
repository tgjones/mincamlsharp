namespace MinCamlSharp.Compiler

module Map =
    let mapRange f m = Map.map (fun k v -> f(v)) m
    let add_list xys env = List.fold (fun env (x, y) -> Map.add x y env) env xys
    let add_list2 xs ys env = List.fold2 (fun env x y -> Map.add x y env) env xs ys

module Set =
    let of_list l = List.fold (fun s e -> Set.add e s) Set.empty l