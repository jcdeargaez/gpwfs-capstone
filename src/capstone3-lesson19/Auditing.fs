module Capstone3.Auditing

open Capstone3.Domain

/// Logs to the console
let private printTransaction ownerName _ tx =
    let operation = Operation.toString tx.Operation
    printfn "Account %s: %s of $%M (approved: %b)" ownerName operation tx.Amount tx.Accepted

/// Logs transactions into console and file system
let composedLogger =
    let loggers = [
        FileRepository.writeTransaction
        printTransaction
    ]
    fun ownerName accountId transaction ->
        loggers
        |> List.iter (fun logger -> logger ownerName accountId transaction)
