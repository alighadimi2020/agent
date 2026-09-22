using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Ario Tjarat Agent";
});

builder.Services.AddHostedService<AgentWorker>();

var host = builder.Build();
await host.RunAsync();
