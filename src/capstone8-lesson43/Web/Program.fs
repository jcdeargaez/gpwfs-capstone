module Capstone8.App

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open System.Configuration
open Giraffe

open Capstone8.Domain

[<CLIMutable>]
type CustomerReq =
    { Name : string }

[<CLIMutable>]
type TransactionReq =
    { Owner : string
      Amount : decimal
      Operation : BankOperation }

let asOk obj = Successful.ok (json obj)
let asBadRequest err = RequestErrors.badRequest (text err)

let getAccount (bankApi : IBankApi) customer =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! account = bankApi.LoadAccount { Name = customer.Name }
            return! asOk account next ctx
        }

let getTransactions (bankApi : IBankApi) customer =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! transactions = bankApi.LoadTransactionHistory { Name = customer.Name }
            return! asOk transactions next ctx
        }

let postTransaction (bankApi : IBankApi) transaction =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! response = task {
                match transaction.Operation with
                | Deposit ->
                    let! account = bankApi.Deposit transaction.Amount { Name = transaction.Owner }
                    return asOk account
                | Withdraw ->
                    let! withdrawResult = bankApi.Withdraw transaction.Amount { Name = transaction.Owner }
                    match withdrawResult with
                    | Ok account -> return asOk account
                    | Error err -> return asBadRequest err
            }
            return! response next ctx
        }

let accountRouter (bankApi : IBankApi) =
    choose [
        GET >=> route "/account"
            >=> tryBindQuery<CustomerReq> asBadRequest None (getAccount bankApi)
    ]

let transactionRouter (bankApi : IBankApi) =
    choose [
        GET >=> route "/transaction"
            >=> tryBindQuery<CustomerReq> asBadRequest None (getTransactions bankApi)
        POST >=> route "/transaction"
             >=> tryBindForm<TransactionReq> asBadRequest None (postTransaction bankApi)
    ]

let apiRouter (bankApi : IBankApi) =
    choose [
        accountRouter bankApi
        transactionRouter bankApi
    ]

let webApp (bankApi : IBankApi) =
    choose [
        subRoute "/api" (apiRouter bankApi)
        setStatusCode 404 >=> text "Not found"
    ]

let configureServices (services : IServiceCollection) =
    services.AddGiraffe() |> ignore

let configureApp (bankApi : IBankApi) (app : IApplicationBuilder) =
    app.UseGiraffe (webApp bankApi)

[<EntryPoint>]
let main _ =
    let bankApi = BankApi.BuildWithSqlRepository ConfigurationManager.ConnectionStrings["BankAccountDB"].ConnectionString
    
    Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(fun webHostBuilder ->
            webHostBuilder
                .ConfigureServices(configureServices)
                .Configure(configureApp bankApi)
                |> ignore)
        .Build()
        .Run()
    0
