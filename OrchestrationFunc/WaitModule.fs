namespace OrchestrationFunc

module WaitModule =

    let waitForSeconds (seconds:int) : 
        Async<Result<string,obj>> = 
        async {
            let! time = Async.Sleep(seconds)
            return Ok "Success"
        }