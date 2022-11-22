module Capstone8.PropertyTests

open FsCheck
open FsCheck.Xunit

open Capstone8.ApiTests
open Capstone8.Domain

[<Property>]
let ``Going under 0 makes the account overdrawn`` (PositiveInt startingBalance) =
    let startingBalance = decimal startingBalance
    let bankApi = Helper.createInMemApi ()
    async {
        do! bankApi.Deposit startingBalance customer |> Async.Ignore
        let! result = bankApi.Withdraw (startingBalance + 1M) customer
        match result with
        | Ok (Overdrawn _) -> return true
        | _ -> return false
    }
    |> Async.RunSynchronously

[<Property>]
let ``Withdrawal fails if the account is overdrawn`` (PositiveInt withdrawAmount) =
    let bankApi = Helper.createInMemApi ()
    async {
        do! bankApi.Withdraw (decimal withdrawAmount) customer |> Async.Ignore
        let! result = bankApi.Withdraw 1M customer
        match result with
        | Error _ -> return true
        | _ -> return false
    }
    |> Async.RunSynchronously
