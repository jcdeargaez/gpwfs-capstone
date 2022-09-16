module Capstone4.Auditing

open Capstone4.Domain

/// Logs to the console
let private printTransaction ownerName _ tx =
    let operation = BankOperation.toString tx.Operation
    printfn "Account %s: %s of $%M" ownerName operation tx.Amount

/// Logs transactions into console and file system
let composedLogger =
    let loggers = [
        FileRepository.writeTransaction
        printTransaction
    ]
    fun ownerName accountId transaction ->
        loggers
        |> List.iter (fun logger -> logger ownerName accountId transaction)
