namespace Capstone5.Domain

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
      Operation : string
      Amount : decimal }

type Command =
    | AccountCommand of BankOperation
    | Exit
