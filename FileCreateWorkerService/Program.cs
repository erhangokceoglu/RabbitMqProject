using FileCreateWorkerService;
using RabbitMQ.Client;
using RabbitMqCreateExcel.Web.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton(x => new ConnectionFactory()
        {
            Uri = new Uri(hostContext.Configuration.GetConnectionString("RabbitMq")!
        ),
            DispatchConsumersAsync = true
        });

        services.AddSingleton<RabbitMqClientService>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
