using Grpc.Core;
using SharedContracts.Grpc;

namespace ServiceC.Services;

public class TransactionGrpcService : TransactionService.TransactionServiceBase
{
    public override Task<GrpcResponse> ExecuteSyncTask(GrpcRequest request, ServerCallContext context)
    {
        return Task.FromResult(new GrpcResponse
        {
            Success = true,
            Message = "Service C completed successfully via gRPC"
        });
    }
}