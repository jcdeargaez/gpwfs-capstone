open System
open Capstone4
open Capstone4.Domain

let depositWithAudit = Account.auditAs Deposit Auditing.composedLogger
let withdrawWithAudit = Account.auditAs Withdraw Auditing.composedLogger

let displayBalance firstWord account =
    account
    |> Account.iter (fun a -> printfn "%s balance is $%M" firstWord a.Balance)
    match account with
    | Overdrawn _ -> printfn "Your account is overdrawn!!"
    | _ -> ()

let displayCurrentBalance = displayBalance "Current"

let loadAccount () =
    printf "Please enter your name: "
    let ownerName = Console.ReadLine()
    let account =
        match FileRepository.tryFindAccountId ownerName with
        | Some accountId -> Account.restoreAccount (ownerName, accountId)
        | None -> Account.create ownerName None
    account |> displayBalance "Opening"
    account

let commands =
    Seq.initInfinite (fun _ ->
        printf "Select a command (d)eposit, (w)ithdraw or e(x)it: "
        let cmd = Console.ReadKey().KeyChar
        printfn ""
        cmd)

let isStopCommand = (=) Exit

let isValidCommand cmd =
    if cmd = 'x' then Some Exit
    else
        match BankOperation.tryParse cmd with
        | Some operation -> Some (AccountCommand operation)
        | None ->
            printfn "Invalid command"
            None

let tryGetBankOperation cmd =
    match cmd with
    | AccountCommand op -> Some op
    | Exit -> None

let tryGetAmount cmd =
    printf "Enter amount: "
    match Console.ReadLine() |> System.Decimal.TryParse with
    | false, _ ->
        printfn "Invalid amount"
        None
    | true, amount when amount <= 0M ->
        printfn "Invalid amount"
        None
    | true, amount -> Some (cmd, amount)

let processCommand account (operation, amount) =
    printfn ""
    let operationResult =
        match operation with
        | Deposit -> depositWithAudit amount account
        | Withdraw -> withdrawWithAudit amount account
    match operationResult with
    | Ok ratedAccount ->
        displayCurrentBalance ratedAccount
        ratedAccount
    | Error err ->
        printfn "%s" err
        displayCurrentBalance account
        account

[<EntryPoint>]
let main _ =
    let openingAccount = loadAccount ()
    let closingAccount =
        commands
        |> Seq.choose isValidCommand
        |> Seq.takeWhile (not << isStopCommand)
        |> Seq.choose tryGetBankOperation
        |> Seq.choose tryGetAmount
        |> Seq.fold processCommand openingAccount
    printfn ""
    closingAccount |> displayBalance "Closing"
    Console.ReadKey() |> ignore
    0
