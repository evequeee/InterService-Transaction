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
        try
        {
            var taskA = _clientA.GetResponse<WorkAResult>(new DoWorkA());
            var taskB = _clientB.GetResponse<WorkBResult>(new DoWorkB());

            await Task.WhenAll(taskA, taskB);

            var resultA = taskA.Result.Message;
            var resultB = taskB.Result.Message;

            if (!resultA.Success || !resultB.Success)
            {
                return BadRequest(new { Status = "Transaction failed.", Error = "One of the async steps failed."});
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
        catch (MassTransit.RequestTimeoutException)
        {
            return StatusCode(504, new
            {
                Status = "Transaction Failed",
                Error = "Timeout waiting for responses from internal services. One of the services might be down."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Status = "Transaction Failed", Error = ex.Message });
        }
    }
}