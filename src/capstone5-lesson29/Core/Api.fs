/// Provides access to the banking API.
module Capstone5.Api

open Capstone5.Domain

let private depositWithAudit = Account.auditAs (BankOperation.toString Deposit) Auditing.composedLogger
let private withdrawWithAudit = Account.auditAs (BankOperation.toString Withdraw) Auditing.composedLogger

/// Restores an account from its transactions history
let private restoreAccount = FileRepository.loadTransactions >> Account.loadAccount

/// Loads an account from disk. If no account exists, an empty one is automatically created.
let LoadAccount customer =
    match FileRepository.tryFindAccountId customer.Name with
    | Some accountId -> restoreAccount (customer.Name, accountId)
    | None -> Account.create customer.Name None

/// Deposits funds into an account.
let Deposit amount customer =
    let account = LoadAccount customer
    let result =
        account
        |> depositWithAudit amount
    match result with
    | Ok account -> account
    | Error _ -> account

/// Withdraws funds from an account that is in credit.
let Withdraw amount customer =
    let account = LoadAccount customer
    let result =
        account
        |> withdrawWithAudit amount
    match result with
    | Ok account -> account
    | Error _ -> account

/// Loads the transaction history for an owner. If no transactions exist, returns an empty sequence.
let LoadTransactionHistory customer =
    match FileRepository.tryFindAccountId customer.Name with
    | Some accountId ->
        let (_, _, transactions) = FileRepository.loadTransactions (customer.Name, accountId)
        transactions
    | None -> Seq.empty
