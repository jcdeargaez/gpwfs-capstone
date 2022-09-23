module Capstone5.BankOperation

open Capstone5.Domain

/// Parses a bank operation from a string. Fails on invalid input
let parse input =
    match input with
    | "deposit" -> Deposit
    | "withdraw" -> Withdraw
    | _ -> failwith $"Unknown bank operation to parse '{input}'"

/// Parses a bank operation from a character. Returns None on invalid character
let tryParse input =
    match input with
    | "deposit" -> Some Deposit
    | "withdraw" -> Some Withdraw
    | _ -> None

/// Converts the bank operation name to a lower case string
let toString operation =
    match operation with
    | Deposit -> "deposit"
    | Withdraw -> "withdraw"
