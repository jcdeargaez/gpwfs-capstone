module Capstone7.App

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open System.Configuration
open Giraffe

open Capstone7.Domain

[<CLIMutable>]
type CustomerReq =
    { Name : string }

[<CLIMutable>]
type TransactionReq =
    { Owner : string
      Amount : decimal
      Operation : BankOperation }

let bankApi = BankApi.BuildWithSqlRepository ConfigurationManager.ConnectionStrings["BankAccountDB"].ConnectionString

let asOk obj = json obj
let asBadRequest err = RequestErrors.badRequest (text err)

let getAccount customer =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! account = bankApi.LoadAccount { Name = customer.Name }
            return! asOk account next ctx
        }

let getTransactions customer =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! transactions = bankApi.LoadTransactionHistory { Name = customer.Name }
            return! asOk transactions next ctx
        }

let postTransaction transaction =
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

let accountRouter =
    choose [
        GET >=> route "/account"
            >=> tryBindQuery<CustomerReq> asBadRequest None getAccount
    ]

let transactionRouter =
    choose [
        GET >=> route "/transaction"
            >=> tryBindQuery<CustomerReq> asBadRequest None getTransactions
        POST >=> route "/transaction"
             >=> tryBindForm<TransactionReq> asBadRequest None postTransaction
    ]

let apiRouter =
    choose [
        accountRouter
        transactionRouter
    ]

let webApp =
    choose [
        subRoute "/api" apiRouter
        setStatusCode 404 >=> text "Not found"
    ]

let configureServices (services : IServiceCollection) =
    services.AddGiraffe() |> ignore

let configureApp (app : IApplicationBuilder) =
    app.UseGiraffe webApp

[<EntryPoint>]
let main _ =
    Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(fun webHostBuilder ->
            webHostBuilder
                .ConfigureServices(configureServices)
                .Configure(configureApp)
                |> ignore)
        .Build()
        .Run()
    0
