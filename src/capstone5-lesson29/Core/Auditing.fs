module Capstone5.Auditing

open Capstone5.Domain
open Capstone5.BankOperation

/// Logs to the console
let private printTransaction ownerName _ tx =
    printfn "Account %s: %s of $%M" ownerName tx.Operation tx.Amount

/// Logs transactions into console and file system
let composedLogger =
    let loggers = [
        FileRepository.writeTransaction
        printTransaction
    ]
    fun ownerName accountId transaction ->
        loggers
        |> List.iter (fun logger -> logger ownerName accountId transaction)
