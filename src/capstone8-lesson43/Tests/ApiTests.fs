module Capstone8.ApiTests

open Swensen.Unquote.Assertions
open Xunit

open Capstone8.Domain
open Capstone8.Helper

let customer = { Name = "Joe" }

[<Fact>]
let ``Creates an account if none exists`` () =
    let api = createInMemApi ()
    async {
        let! loadedAccount = api.LoadAccount customer
        verifyBalance 0M loadedAccount
    }

[<Fact>]
let ``Multiple deposits are stored correctly`` () =
    let api = createInMemApi ()
    async {
        do! api.Deposit 10M customer |> Async.Ignore
        let! ratedAccount = api.Deposit 20M customer
        verifyBalance 30M ratedAccount
    }

[<Fact>]
let ``Cannot withdraw if overdrawn`` () =
    let api = createInMemApi ()
    async {
        do! api.Withdraw 10M customer |> Async.Ignore
        let! result = api.Withdraw 1M customer
        test <@ Result.isError result @>
    }
