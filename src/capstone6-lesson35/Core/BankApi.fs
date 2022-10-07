/// Provides access to the banking API.
module Capstone6.BankApi

open Capstone6.Domain

let private loadAccount getAccountAndTransactions customer =
    customer.Name
    |> (getAccountAndTransactions >> Account.loadAccount)

let private loadTransactionHistory getAccountAndTransactions customer =
    let (_, _, transactions) = getAccountAndTransactions customer.Name
    transactions

let private deposit getAccountAndTransactions writeTransaction amount customer =
    let account = loadAccount getAccountAndTransactions customer
    account
    |> Account.auditAs Deposit writeTransaction amount
    |> Result.defaultValue account

let private withdraw getAccountAndTransactions writeTransaction amount customer =
    let account = loadAccount getAccountAndTransactions customer
    account
    |> Account.auditAs Withdraw writeTransaction amount
    |> Result.defaultValue account

let private buildApi getAccountAndTransactions writeTransaction =
    { new IBankApi with
        member this.LoadAccount customer = loadAccount getAccountAndTransactions customer
        member this.LoadTransactionHistory customer = loadTransactionHistory getAccountAndTransactions customer
        member this.Deposit amount customer = deposit getAccountAndTransactions writeTransaction amount customer
        member this.Withdraw amount customer = withdraw getAccountAndTransactions writeTransaction amount customer }

/// Builds a bank API with a SQL repository
let BuildWithSqlRepository connectionString =
    buildApi (SqlRepository.getAccountAndTransactions connectionString) (SqlRepository.writeTransaction connectionString)

/// Builds a bank API with a file system repository
let BuildWithFileSystemRepository () = buildApi FileRepository.loadTransactions FileRepository.writeTransaction
