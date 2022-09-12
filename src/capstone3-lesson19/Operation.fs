module Capstone3.Operation

open Capstone3.Domain

let parse input =
    match input with
    | 'd' -> Deposit
    | 'w' -> Withdraw
    | _ -> failwith $"Unknown operation to parse '{input}'"

let tryParse input =
    match input with
    | 'd' -> Some Deposit
    | 'w' -> Some Withdraw
    | _ -> None

let toString operation =
    match operation with
    | Deposit -> "deposit"
    | Withdraw -> "withdraw"

let toChar operation =
    match operation with
    | Deposit -> 'd'
    | Withdraw -> 'w'
