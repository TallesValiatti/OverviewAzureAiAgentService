using Azure;
using Azure.AI.Projects;
using Azure.Identity;
using OverviewAzureAiAgentService.Api.Services.Models;
using Agent = OverviewAzureAiAgentService.Api.Services.Models.Agent;

namespace OverviewAzureAiAgentService.Api.Services;

public class AgentService(IConfiguration configuration)
{
  public async Task<Agent> CreateAgentAsync(CreateAgentRequest request)
  {
    var connectionString = configuration["AiServiceProjectConnectionString"]!;
    var aiModel = configuration["AiModel"]!;

    AgentsClient client = new AgentsClient(connectionString, new DefaultAzureCredential());
    
    var agentResponse = await client.CreateAgentAsync(
      model: aiModel,
      name: request.Name,
      instructions: request.Instructions);
    
    return new Agent(
      agentResponse.Value.Id,
      agentResponse.Value.Name,
      agentResponse.Value.Instructions);
  }
}