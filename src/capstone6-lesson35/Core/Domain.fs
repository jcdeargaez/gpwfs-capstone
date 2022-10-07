namespace Capstone6.Domain

open System

type Customer =
    { Name : string }

type Account =
    { AccountId : Guid
      Owner : Customer
      Balance : decimal }

type CreditAccount = CreditAccount of Account

type RatedAccount =
    | InCredit of CreditAccount
    | Overdrawn of Account
    
    member this.Balance =
        match this with
        | InCredit (CreditAccount account) -> account.Balance
        | Overdrawn account -> account.Balance

type BankOperation = Deposit | Withdraw

type Transaction =
    { Timestamp : DateTime
      Operation : BankOperation
      Amount : decimal }

/// OO interface for better interoperability exposing the API to C# users
type IBankApi =
    /// Loads an account from disk. If no account exists, an empty one is returned.
    abstract member LoadAccount : Customer -> RatedAccount

    /// Loads the transaction history for an owner. If no transactions exist, returns an empty sequence.
    abstract member LoadTransactionHistory : Customer -> Transaction seq

    /// Deposits funds into an account.
    abstract member Deposit : decimal -> Customer -> RatedAccount

    /// Withdraws funds from an account that is in credit.
    abstract member Withdraw: decimal -> Customer -> RatedAccount
