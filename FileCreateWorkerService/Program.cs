using FileCreateWorkerService;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<AdventureWorks2022Context>(options =>
        {
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("SqlServer"));
        });

        services.AddSingleton(x => new ConnectionFactory()
        {
            Uri = new Uri(hostContext.Configuration.GetConnectionString("RabbitMq")!),
            DispatchConsumersAsync = true
        });

        services.AddSingleton<RabbitMqClientService>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
