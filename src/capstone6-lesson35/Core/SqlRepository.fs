module Capstone6.SqlRepository

open FSharp.Data
open Capstone6.Domain

[<AutoOpen>]
module private DB =
    let [<Literal>] Conn = "Data Source=(localdb)\MSSQLLocalDB;Database=BankAccountDB;Integrated Security=True;Connect Timeout=60"
    type BankAccountsDB = SqlProgrammabilityProvider<Conn>
    type GetAccountId = SqlCommandProvider<"SELECT TOP 1 AccountId FROM dbo.Account WHERE Owner = @owner", Conn, SingleRow = true>
    type FindTransactions = SqlCommandProvider<"SELECT Timestamp, OperationId, Amount FROM dbo.AccountTransaction WHERE AccountId = @accountId", Conn>
    type InsertAccount = SqlCommandProvider<"INSERT INTO dbo.Account (Owner, AccountId) VALUES (@owner, @accountId)", Conn>
    type InsertTransaction = SqlCommandProvider<"INSERT INTO dbo.AccountTransaction (AccountId, Timestamp, OperationId, Amount) VALUES (@accountId, @timestamp, @operationId, @amount)", Conn>
    type OperationId = SqlEnumProvider<"SELECT Description, OperationId FROM dbo.Operation", Conn>

/// Returns a bank operation given an operation id from the Operation table in the DB
let toBankOperation =
    function
    | DB.OperationId.Withdraw -> Withdraw
    | DB.OperationId.Deposit -> Deposit
    | _ -> failwith $"Unknown operation id"

/// Queries all transactions for an account
let getTransactions (connectionString : string) accountId =
    DB.FindTransactions.Create(connectionString).Execute(accountId)
    |> Seq.map (fun record -> { Timestamp = record.Timestamp; Operation = toBankOperation record.OperationId; Amount = record.Amount })

/// Queries the accountId and transactions given the owner name.
/// Returns a tuple of the owner name, optional accountId and transactions sequence
let getAccountAndTransactions (connectionString : string) owner =
    match DB.GetAccountId.Create(connectionString).Execute(owner) with
    | Some accountId -> owner, Some accountId, getTransactions connectionString accountId
    | None -> owner, None, Seq.empty

/// Inserts an account if an owner does not have one yet
let writeAccount (connectionString : string) ownerName accountId =
    try
        DB.InsertAccount.Create(connectionString).Execute(ownerName, accountId) |> ignore
        Ok ()
    with
    | ex -> Error ex.Message

/// Records a transaction on an account. If the account does not exist yet, it is created
let writeTransaction (connectionString : string) ownerName accountId transaction =
    match writeAccount connectionString ownerName accountId with
    | Error err when not (err.Contains "Violation of PRIMARY KEY") -> failwith $"Failure writing account: {err}"
    | _ -> ()

    let operationId =
        match transaction.Operation with
        | Withdraw -> DB.OperationId.Withdraw
        | Deposit -> DB.OperationId.Deposit

    DB.InsertTransaction
        .Create(connectionString)
        .Execute(accountId, transaction.Timestamp, operationId, transaction.Amount) |> ignore
