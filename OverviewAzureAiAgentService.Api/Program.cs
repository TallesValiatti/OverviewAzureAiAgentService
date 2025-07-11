using Microsoft.AspNetCore.Mvc;
using OverviewAzureAiAgentService.Api.Services;
using OverviewAzureAiAgentService.Api.Services.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddScoped<AgentService>();
builder.Services.AddSingleton<EmailService>(x => new EmailService(
    builder.Configuration["Email:Secret"]!, 
    builder.Configuration["Email:SenderEmail"]!));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

app.MapOpenApi();

app.UseHttpsRedirection();

app.MapPost("/agents", async ([FromServices] AgentService service, CreateAgentRequest request) =>
{
    Agent agent;
    
    if (request.IsSalesAgent)
    {
        agent = await service.CreateSalesAgentAsync(request);    
    }
    else if(request.IsDocAgent)
    {
        agent = await service.CreateDocAgentAsync(request);
    }
    else if(request.IsEmailSenderAgent)
    {
        agent = await service.CreateEmailSenderAgentAsync(request);
    }
    else if (request.IsHistoryAgent)
    {
        agent = await service.CreateHistoryAgentAsync(request);
    }
    else
    {
        agent = await service.CreateAgentAsync(request);
    }
    
    return Results.Ok(agent);
}).WithName("CreateAgent");

app.MapPost("/threads", async ([FromServices] AgentService service) => 
    await service.CreateThreadAsync())
    .WithName("CreateThread");

app.MapGet("/threads/{threadId}/messages", async ([FromServices] AgentService service, string threadId) =>
    await service.ListMessagesAsync(threadId))
    .WithName("ListThreadMessages");

app.MapPost("/run", async ([FromServices] AgentService service, CreateRunRequest request) =>
    await service.CreateRunAsync(request))
    .WithName("CreateRun");
    
app.Run();