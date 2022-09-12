namespace Capstone3.Domain

open System

type Customer = {
    Name: string
}

type Account = {
    AccountId: Guid
    Owner: Customer
    Balance: decimal
}

type Operation =
    | Deposit
    | Withdraw

type Transaction = {
    Timestamp: DateTime
    Operation: Operation
    Amount: decimal
    Accepted: bool
}
