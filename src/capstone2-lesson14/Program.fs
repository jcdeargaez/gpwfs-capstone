open System
open Capstone2
open Capstone2.Domain
open Capstone2.Operations

let openAccount () =
    printf "Enter your name: "
    let name = Console.ReadLine()
    printf "Enter your opening balance: "
    let balance = Console.ReadLine()
    { AccountId = Guid.NewGuid()
      Owner = { Name = name }
      Balance = decimal balance }

let captureOperation account =
    printfn "Current balance is $%M" account.Balance
    printf "Enter operation (d)eposit, (w)ithdraw or e(x)it: "
    Console.ReadLine()

let captureAmount () =
    printf "Enter amount: "
    Console.ReadLine() |> decimal

let depositWithAudit = deposit |> auditAs "deposit" Auditing.fileSystem
let withdrawWithAudit = withdraw |> auditAs "withdraw" Auditing.fileSystem

let rec inputLoop account =
    let operation = captureOperation account
    if operation = "x" then
        Environment.Exit 0

    let account' =
        match operation with
        | "d" ->
            let amount = captureAmount()
            account |> depositWithAudit amount
        | "w" ->
            let amount = captureAmount()
            account |> withdrawWithAudit amount
        | _ ->
            printfn "Invalid operation"
            account
    
    inputLoop account'

openAccount () |> inputLoop
