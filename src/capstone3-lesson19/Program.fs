open System
open Capstone3
open Capstone3.Domain

let depositWithAudit = Account.auditAs Deposit Auditing.composedLogger
let withdrawWithAudit = Account.auditAs Withdraw Auditing.composedLogger

let loadAccount () =
    printf "Please enter your name: "
    let ownerName = Console.ReadLine()
    let account =
        match FileRepository.tryFindAccountId ownerName with
        | Some accountId ->
            FileRepository.loadTransactions ownerName accountId
            |> Account.loadAccount ownerName accountId
        | None -> Account.create ownerName
    printfn "Current balance is $%M" account.Balance
    account

let commands =
    seq {
        while true do
            printf "Select a command (d)eposit, (w)ithdraw or e(x)it: "
            let cmd = Console.ReadKey().KeyChar
            printfn ""
            yield cmd
    }

let isStopCommand = (=) 'x'

let isValidCommand cmd =
    match Operation.tryParse cmd with
    | Some _ -> true
    | None ->
        printfn "Invalid command"
        false

let getAmount operation =
    printf "Enter amount: "
    let amount = Console.ReadLine() |> decimal
    operation, amount

let processOperation account (operation, amount) =
    printfn ""
    let account' =
        match operation with
        | Deposit -> depositWithAudit amount account
        | Withdraw -> withdrawWithAudit amount account
    printfn "Current balance is $%M" account'.Balance
    account'

[<EntryPoint>]
let main _ =
    let openingAccount = loadAccount ()
    let closingAccount =
        commands
        |> Seq.takeWhile (not << isStopCommand)
        |> Seq.filter isValidCommand
        |> Seq.map Operation.parse
        |> Seq.map getAmount
        |> Seq.fold processOperation openingAccount
    printfn ""
    printfn "Closing balance is $%M" closingAccount.Balance
    Console.ReadKey() |> ignore
    0
