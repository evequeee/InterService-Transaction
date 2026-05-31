using MassTransit;
using SharedContracts;

namespace ServiceA.Consumers;

public class WorkAConsumer : IConsumer<DoWorkA>
{
    public async Task Consume(ConsumeContext<DoWorkA> context)
    {
        await Task.Delay(300);
        await context.RespondAsync(new WorkAResult(true, "Service A inner task completed"));
    }
}