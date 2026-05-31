using MassTransit;
using SharedContracts.Grpc;
using ServiceA.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<WorkAConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddGrpcClient<TransactionService.TransactionServiceClient>(o =>
{
    o.Address = new Uri("http://localhost:5271"); 
});

var app = builder.Build();
app.MapControllers();
app.Run();