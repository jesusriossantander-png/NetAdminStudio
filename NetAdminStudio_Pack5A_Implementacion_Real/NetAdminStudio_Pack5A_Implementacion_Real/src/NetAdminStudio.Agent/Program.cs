using NetAdminStudio.Agent;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5188");
    client.Timeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddHostedService<Worker>();

await builder.Build().RunAsync();
