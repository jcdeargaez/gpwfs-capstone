namespace Capstone4.Domain

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

type Command =
    | AccountCommand of BankOperation
    | Exit
