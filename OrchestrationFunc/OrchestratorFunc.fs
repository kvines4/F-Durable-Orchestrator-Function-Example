namespace OrchestrationFunc

open System;
open System.Net.Http;

open Microsoft.Azure.WebJobs;
open Microsoft.Azure.WebJobs.Extensions.DurableTask;
open Microsoft.Azure.WebJobs.Extensions.Http;
open Microsoft.Extensions.Logging;

open FsToolkit.ErrorHandling

module LxpOrchestrator =

    [<FunctionName("HttpOrchestrator")>]
    let Run ([<HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orchestrators/{functionName}")>] req: HttpRequestMessage,
             [<DurableClient>] starter: IDurableOrchestrationClient, functionName: string, log: ILogger) =
        
        async {
            // get POST body
            let eventData = 
                async {
                    let! x = 
                        req.Content.ReadAsAsync<Object>() 
                        |> Async.AwaitTask 
                    return x
                } |> Async.RunSynchronously
            
            // Start instance & generate an ID
            let! instanceId = 
                starter.StartNewAsync(functionName, eventData) 
                |> Async.AwaitTask

            log.LogInformation(sprintf "Started orchestration with ID = '{%s}'." instanceId)

            return starter.CreateCheckStatusResponse(req, instanceId)
        } |> Async.StartAsTask