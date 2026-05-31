using MassTransit;
using SharedContracts;

namespace ServiceB.Consumers;

public class WorkBConsumer : IConsumer<DoWorkB>
{
    public async Task Consume(ConsumeContext<DoWorkB> context)
    {
        await Task.Delay(500);
        
        await context.RespondAsync(new WorkBResult(true, "Service B task completed"));
    }
}