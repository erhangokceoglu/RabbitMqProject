using FileCreateWorkerService;
using FileCreateWorkerService.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMqCreateExcel.Web.Contexts;
using RabbitMqCreateExcel.Web.Services;
using System.Reflection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<AdventureWorks2022Context>(options =>
        {
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("SqlServer"), options =>
            {
            });
        });

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
