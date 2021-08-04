namespace OrchestrationFunc

open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.DurableTask
open Microsoft.Extensions.Logging

open FsToolkit.ErrorHandling

open WaitModule

module Funcs =

    // Modify parameters class to match expected JSON body
    type Parameters() =
        member val waitTime:int = 1000 with get, set

    [<FunctionName("WaitFunc")>]
    let WaitFunc ([<OrchestrationTrigger>]context: IDurableOrchestrationContext) (log: ILogger) =
        log.LogInformation("WaitFunc initiated...")

        // Get Parameters
        let (parameters:Parameters) = context.GetInput<Parameters>()

        // Do something
        let result = 
            async { 
                let! x = waitForSeconds parameters.waitTime
                return x
            } |> Async.RunSynchronously

        // Return result
        match result with
        | Error err -> failwith (err.ToString())
        | Ok result -> (OkObjectResult(result) :> IActionResult)