module Capstone3.FileRepository

open System
open System.IO

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
let tryFindAccountId ownerName =
    let dirPattern = sprintf "%s_*" ownerName
    let dirs = Directory.EnumerateDirectories(accountsPath, dirPattern)
    if Seq.isEmpty dirs then None
    else
        let accountId =
            let dirPath = Seq.head dirs
            let parts = DirectoryInfo(dirPath).Name.Split('_')
            Guid.Parse parts.[1]
        Some accountId

/// Extracts all transactions history from an account directory
let loadTransactions ownerName accountId =
    buildAccountDirectoryPath ownerName accountId
    |> Directory.EnumerateFiles
    |> Seq.map (File.ReadAllText >> Transaction.deserialize)

/// Logs a transaction in its own file in the account directory
let writeTransaction ownerName accountId transaction =
    let dirPath = buildAccountDirectoryPath ownerName accountId
    let filePath =
        let fileName = sprintf "%d.txt" (DateTime.UtcNow.ToFileTimeUtc())
        Path.Join [| dirPath; fileName |]
    let message = Transaction.serialize transaction
    Directory.CreateDirectory dirPath |> ignore
    File.WriteAllText(filePath, message)
