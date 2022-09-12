module Capstone3.Account

open System
open Capstone3.Domain

let create ownerName =
    { Owner = { Name = ownerName }
      AccountId = Guid.NewGuid()
      Balance = 0M }

/// Deposits an amount into an account
let deposit amount account =
    { account with Balance = account.Balance + amount }

/// Withdraws an amount of an account (if there are sufficient funds)
let withdraw amount account =
    if amount > account.Balance then account
    else { account with Balance = account.Balance - amount }

/// Runs some account operation such as withdraw or deposit with auditing
let auditAs operation audit amount account =
    let f =
        match operation with
        | Deposit -> deposit
        | Withdraw -> withdraw
    let account' = f amount account
    let tx = {
        Timestamp = DateTime.UtcNow
        Operation = operation
        Amount = amount
        Accepted = account'.Balance <> account.Balance
    }
    audit account.Owner.Name account.AccountId tx
    account'

/// Restores an account from its transactions history
let loadAccount ownerName accountId transactions =
    let loadTransaction accountSoFar tx =
        match tx.Operation with
        | Deposit -> deposit tx.Amount accountSoFar
        | Withdraw -> withdraw tx.Amount accountSoFar
    let initialAccount =
        { Owner = { Name = ownerName }
          AccountId = accountId
          Balance = 0M }
    transactions
    |> Seq.filter (fun tx -> tx.Accepted)
    |> Seq.sortBy (fun tx -> tx.Timestamp)
    |> Seq.fold loadTransaction initialAccount
