module Capstone6.FileRepository

open System
open System.IO
open Newtonsoft.Json
open Capstone6.Domain

/// Path where accounts files are stored
let private accountsPath =
    let path = Path.Join [| Directory.GetCurrentDirectory(); "tmp" ; "accounts" |]
    Directory.CreateDirectory path |> ignore
    path

/// Builds directory path for an account,
/// only one account by customer is supported
let private buildAccountDirectoryPath ownerName (accountId:Guid) =
    let dirName = sprintf "%s_%O" ownerName accountId
    Path.Join [| accountsPath; dirName |]

/// Finds the accountId for an owner if exists
let private tryFindAccountId ownerName =
    let extractAccountId path =
        let accountId =
            let parts = DirectoryInfo(path).Name.Split('_')
            Guid.Parse parts.[1]
        Some accountId
    let dirPattern = sprintf "%s_*" ownerName
    Directory.EnumerateDirectories(accountsPath, dirPattern)
    |> Seq.tryHead
    |> Option.bind extractAccountId

/// Extracts all transactions history from an account directory
let loadTransactions ownerName =
    match tryFindAccountId ownerName with
    | Some accountId ->
        let transactions =
            buildAccountDirectoryPath ownerName accountId
            |> Directory.EnumerateFiles
            |> Seq.map (File.ReadAllText >> JsonConvert.DeserializeObject<Transaction>)
        ownerName, Some accountId, transactions
    | None -> ownerName, None, Seq.empty

/// Logs a transaction in its own file in the account directory
let writeTransaction ownerName accountId transaction =
    let dirPath = buildAccountDirectoryPath ownerName accountId
    let filePath =
        let fileName = sprintf "%d.txt" (DateTime.UtcNow.ToFileTimeUtc())
        Path.Join [| dirPath; fileName |]
    let message = JsonConvert.SerializeObject transaction
    Directory.CreateDirectory dirPath |> ignore
    File.WriteAllText(filePath, message)
