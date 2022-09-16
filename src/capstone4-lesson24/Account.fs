module Capstone4.Account

open System
open Capstone4.Domain

/// Executes a unit action with the account data
let iter f account : unit =
    let account' =
        match account with
        | InCredit (CreditAccount creditAccount) -> creditAccount
        | Overdrawn overdrawnAccount -> overdrawnAccount
    f account'

/// Executes a function with the account data and returns a rated account
let bind binder account : RatedAccount =
    let account' =
        match account with
        | InCredit (CreditAccount creditAccount) -> creditAccount
        | Overdrawn overdrawnAccount -> overdrawnAccount
    binder account'

/// Rates an account either in credit or overdrawn
let classifyAccount account =
    if account.Balance >= 0M
    then CreditAccount account |> InCredit
    else Overdrawn account

/// Opens a new account with no balance
let create ownerName (accountId: Guid option) =
    let accountId' = defaultArg accountId (Guid.NewGuid())
    { Owner = { Name = ownerName }
      AccountId = accountId'
      Balance = 0M }
    |> classifyAccount

/// Deposits an amount into an account
let deposit amount account =
    account
    |> bind (fun a -> classifyAccount { a with Balance = a.Balance + amount })
    |> Ok

/// Withdraws an amount of an account (if there are sufficient funds)
let private withdraw amount (CreditAccount a) =
    classifyAccount { a with Balance = a.Balance - amount }

/// Withdraws an amount if the rated account is in credit. If the account is already overdrawn returns error
let tryWithdraw amount account =
    match account with
    | InCredit creditAccount ->
        creditAccount
        |> withdraw amount
        |> Ok
    | Overdrawn _ -> Error "You cannot withdraw funds as your account is overdrawn!"

/// Runs some account operation such as withdraw or deposit with auditing
let auditAs bankOperation audit amount account =
    let auditSuccessfulOperation account' =
        account'
        |> iter (fun a ->
            let tx =
                { Timestamp = DateTime.UtcNow
                  Operation = bankOperation
                  Amount = amount }
            audit a.Owner.Name a.AccountId tx)
        Ok account'
    let operation =
        match bankOperation with
        | Deposit -> deposit
        | Withdraw -> tryWithdraw
    operation amount account
    |> Result.bind auditSuccessfulOperation

/// Loads an account from its transactions history
let loadAccount (ownerName, accountId, transactions) =
    let loadTransaction accountSoFar tx =
        let operation =
            match tx.Operation with
            | Deposit -> deposit
            | Withdraw -> tryWithdraw 
        match operation tx.Amount accountSoFar with
        | Ok account -> account
        | Error err -> failwith $"Failure loading transaction: {err}"
    let initialAccount = create ownerName (Some accountId)
    transactions
    |> Seq.sortBy (fun tx -> tx.Timestamp)
    |> Seq.fold loadTransaction initialAccount

/// Restores an account from its transactions history
let restoreAccount = FileRepository.loadTransactions >> loadAccount
