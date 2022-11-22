module Capstone8.Helper

open Swensen.Unquote.Assertions

open System
open System.Collections.Generic

let createInMemApi () =
    let dataStore = Dictionary()

    let load owner =
        match dataStore.TryGetValue owner with
        | true, (accountId, transactions) -> owner, Some accountId, transactions
        | false, _ -> owner, None, Seq.empty
        |> async.Return

    let save owner accountId transaction =
        let details =
            match dataStore.TryGetValue owner with
            | true, (_, transactions) ->
                let transactions' =
                    transactions
                    |> Seq.toList
                    |> List.append [transaction]
                    |> List.toSeq
                accountId, transactions'
            | false, _ -> Guid.NewGuid (), [transaction]
        dataStore.[owner] <- details

    BankApi.buildApi load save

let verifyBalance balance ratedAccount =
    ratedAccount
    |> Account.iter (fun account -> test <@ account.Balance = balance @>)
