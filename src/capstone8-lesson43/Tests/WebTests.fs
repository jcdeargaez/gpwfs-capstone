module Capstone8.WebTests

open System.IO
open System.Text
open System.Threading.Tasks

open Giraffe
open Microsoft.AspNetCore.Http
open Newtonsoft.Json
open NSubstitute
open Swensen.Unquote.Assertions
open Xunit

open Capstone8.App
open Capstone8.Domain
open Capstone8.Helper

let next = Some >> Task.FromResult

let getBody (ctx : HttpContext) =
    ctx.Response.Body.Position <- 0L
    use reader = new StreamReader (ctx.Response.Body, Encoding.UTF8)
    task {
        return! reader.ReadToEndAsync ()
    }

let getJsonBody<'t> (ctx : HttpContext) =
    task {
        let! body = getBody ctx
        return JsonConvert.DeserializeObject<'t> body
    }

let mockContext () =
    let ctx = Substitute.For<HttpContext>()
    ctx.RequestServices.GetService(typeof<Json.ISerializer>).Returns(NewtonsoftJson.Serializer(NewtonsoftJson.Serializer.DefaultSettings)) |> ignore
    ctx.Response.Body <- new MemoryStream()
    ctx

let buildTransactionHandler () = postTransaction (createInMemApi ())

[<Fact>]
let ``Successfull withdrawal returns OK`` () =
    let handler = buildTransactionHandler ()
    let operation = { Owner = "Fred"; Amount = 10M; Operation = Withdraw }

    task {
        let! result = handler operation next (mockContext ())
        test <@ result.IsSome && result.Value.Response.StatusCode = StatusCodes.Status200OK @>
    }

[<Fact>]
let ``Unsuccessfull withdrawal returns 400`` () =
    let handler = buildTransactionHandler ()
    let operation = { Owner = "Fred"; Amount = 10M; Operation = Withdraw }

    task {
        let! _ = handler operation next (mockContext ())
        let! result = handler operation next (mockContext ())
        test <@ result.IsSome && result.Value.Response.StatusCode = StatusCodes.Status400BadRequest @>
    }

[<Fact>]
let ``Returns correct balance`` () =
    let handler = buildTransactionHandler ()
    let verifyOperation operation balance =
        task {
            let! result = handler operation next (mockContext ())
            let! ratedAccount = getJsonBody<RatedAccount> result.Value
            verifyBalance balance ratedAccount    
        }

    let operation1 = { Owner = "Fred"; Amount = 1M; Operation = Deposit }
    let operation2 = { Owner = "Fred"; Amount = 2M; Operation = Deposit }
    let operation3 = { Owner = "Fred"; Amount = 5M; Operation = Withdraw }
    let operation4 = { Owner = "Fred"; Amount = 3M; Operation = Withdraw }

    task {
        do! verifyOperation operation1 1M
        do! verifyOperation operation2 3M
        do! verifyOperation operation3 -2M

        let! result4 = handler operation4 next (mockContext ())
        let! body = getBody result4.Value
        test <@ body.Contains "your account is overdrawn" @>
    }
