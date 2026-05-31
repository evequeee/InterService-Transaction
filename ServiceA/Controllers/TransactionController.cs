using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SharedContracts;
using SharedContracts.Grpc;

namespace ServiceA.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly IRequestClient<DoWorkA> _clientA;
    private readonly IRequestClient<DoWorkB> _clientB;
    private readonly TransactionService.TransactionServiceClient _grpcClient;

    public TransactionController(
        IRequestClient<DoWorkA> clientA,
        IRequestClient<DoWorkB> clientB,
        TransactionService.TransactionServiceClient grpcClient)
    {
        _clientA = clientA;
        _clientB = clientB;
        _grpcClient = grpcClient;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartTransaction()
    {
        var taskA = _clientA.GetResponse<WorkAResult>(new DoWorkA());
        var taskB = _clientB.GetResponse<WorkBResult>(new DoWorkB());

        await Task.WhenAll(taskA, taskB);

        var resultA = taskA.Result.Message;
        var resultB = taskB.Result.Message;

        if (!resultA.Success || !resultB.Success)
        {
            return BadRequest("Transaction failed at asynchronous step.");
        }

        var grpcResponse = await _grpcClient.ExecuteSyncTaskAsync(new GrpcRequest());

        return Ok(new
        {
            Status = "Transaction Completed",
            StepA = resultA.Message,
            StepB = resultB.Message,
            StepC = grpcResponse.Message
        });
    }
}