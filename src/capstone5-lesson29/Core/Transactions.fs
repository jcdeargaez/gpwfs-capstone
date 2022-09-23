module Capstone5.Transactions

open Newtonsoft.Json
open Capstone5.Domain

/// Serializes a transaction
let serialize tx =
    JsonConvert.SerializeObject tx

/// Deserializes a transaction
let deserialize (text:string) =
    JsonConvert.DeserializeObject<Transaction> text
