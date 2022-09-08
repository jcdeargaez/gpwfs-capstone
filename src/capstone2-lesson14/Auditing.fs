module Capstone2.Auditing

open System
open System.IO
open Capstone2.Domain

/// Builds a message to log
let private buildMessage account message =
    sprintf "Account %O - %O: %s\n" account.AccountId DateTime.UtcNow message

/// Logs to the file system
let fileSystem account message =
    let dirPath = Path.Join [| Directory.GetCurrentDirectory(); "tmp" ; "accounts"; account.Owner.Name |]
    let filePath = Path.Join [| dirPath; $"{account.AccountId}.txt" |]
    let msg = buildMessage account message
    Directory.CreateDirectory dirPath |> ignore
    File.AppendAllText(filePath, msg)

/// Logs to the console
let console account message =
    buildMessage account message
    |> printf "%s"
