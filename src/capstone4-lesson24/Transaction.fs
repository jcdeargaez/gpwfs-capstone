module Capstone4.Transaction

open System
open Capstone4.Domain

/// Serializes a transaction
let serialize tx =
    let operation = BankOperation.toChar tx.Operation
    sprintf "%O***%c***%M" tx.Timestamp operation tx.Amount

/// Deserializes a transaction
let deserialize (text:string) =
    let parts = text.Split("***")
    { Timestamp = DateTime.Parse(parts.[0])
      Operation = (char >> BankOperation.parse) parts.[1]
      Amount = decimal parts.[2] }
