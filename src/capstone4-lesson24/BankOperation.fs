module Capstone4.BankOperation

open Capstone4.Domain

/// Parses a bank operation from a character. Fails on invalid character
let parse input =
    match input with
    | 'd' -> Deposit
    | 'w' -> Withdraw
    | _ -> failwith $"Unknown bank operation to parse '{input}'"

/// Parses a bank operation from a character. Returns None on invalid character
let tryParse input =
    match input with
    | 'd' -> Some Deposit
    | 'w' -> Some Withdraw
    | _ -> None

/// Converts the bank operation name to a lower case string
let toString operation =
    match operation with
    | Deposit -> "deposit"
    | Withdraw -> "withdraw"

/// Converts the bank operation name to a lower case character
let toChar operation =
    match operation with
    | Deposit -> 'd'
    | Withdraw -> 'w'
