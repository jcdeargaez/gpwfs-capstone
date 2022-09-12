module Capstone3.Transaction

open System
open Capstone3.Domain

/// Serializes a transaction
let serialize tx =
    let operation = Operation.toChar tx.Operation
    sprintf "%O***%c***%M***%b" tx.Timestamp operation tx.Amount tx.Accepted

/// Deserializes a transaction
let deserialize (text:string) =
    let parts = text.Split("***")
    { Timestamp = DateTime.Parse(parts.[0])
      Operation = (char >> Operation.parse) parts.[1]
      Amount = decimal parts.[2]
      Accepted = Boolean.Parse parts.[3] }
