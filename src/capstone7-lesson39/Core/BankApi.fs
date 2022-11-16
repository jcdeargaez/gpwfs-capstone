/// Provides access to the banking API.
module Capstone7.BankApi

open Capstone7.Domain

let private loadAccount getAccountAndTransactions customer =
    async {
        let! accountDetails = getAccountAndTransactions customer.Name
        return Account.loadAccount accountDetails
    }

let private loadTransactionHistory getAccountAndTransactions customer =
    async {
        let! (_, _, transactions) = getAccountAndTransactions customer.Name
        return transactions
    }

let private deposit getAccountAndTransactions writeTransaction amount customer =
    async {
        let! account = loadAccount getAccountAndTransactions customer
        return
            account
            |> Account.auditAs Deposit writeTransaction amount
            |> Result.defaultValue account
    }

let private withdraw getAccountAndTransactions writeTransaction amount customer =
    async {
        let! account = loadAccount getAccountAndTransactions customer
        return
            account
            |> Account.auditAs Withdraw writeTransaction amount
    }

let private buildApi getAccountAndTransactions writeTransaction =
    { new IBankApi with
        member this.LoadAccount customer = loadAccount getAccountAndTransactions customer
        member this.LoadTransactionHistory customer = loadTransactionHistory getAccountAndTransactions customer
        member this.Deposit amount customer = deposit getAccountAndTransactions writeTransaction amount customer
        member this.Withdraw amount customer = withdraw getAccountAndTransactions writeTransaction amount customer }

let private startWriteTransaction writer ownerName accountId transaction =
    writer ownerName accountId transaction |> Async.Start

/// Builds a bank API with a SQL repository
let BuildWithSqlRepository connectionString =
    buildApi (SqlRepository.getAccountAndTransactions connectionString) (startWriteTransaction (SqlRepository.writeTransaction connectionString))

/// Builds a bank API with a file system repository
let BuildWithFileSystemRepository () = buildApi FileRepository.loadTransactions (startWriteTransaction FileRepository.writeTransaction)
