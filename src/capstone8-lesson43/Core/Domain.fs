namespace Capstone8.Domain

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

type BankOperation = Deposit | Withdraw

type Transaction =
    { Timestamp : DateTime
      Operation : BankOperation
      Amount : decimal }

/// Bank façade for web operations
type IBankApi =
    /// Loads an account from disk. If no account exists, an empty one is returned.
    abstract member LoadAccount : Customer -> Async<RatedAccount>

    /// Loads the transaction history for an owner. If no transactions exist, returns an empty sequence.
    abstract member LoadTransactionHistory : Customer -> Async<Transaction seq>

    /// Deposits funds into an account.
    abstract member Deposit : decimal -> Customer -> Async<RatedAccount>

    /// Withdraws funds from an account that is in credit.
    abstract member Withdraw: decimal -> Customer -> Async<Result<RatedAccount, string>>
