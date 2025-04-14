using System.Text;
using Azure;
using Azure.AI.Projects;
using Azure.Identity;
using OverviewAzureAiAgentService.Api.Services.Models;
using Agent = OverviewAzureAiAgentService.Api.Services.Models.Agent;
using Thread = OverviewAzureAiAgentService.Api.Services.Models.Thread;

namespace OverviewAzureAiAgentService.Api.Services;

public class AgentService(IConfiguration configuration)
{
  private AgentsClient CreateAgentsClient()
  {
    var connectionString = configuration["AiServiceProjectConnectionString"]!;
    return new AgentsClient(connectionString, new DefaultAzureCredential());
  }
  
  public async Task<Agent> CreateAgentAsync(CreateAgentRequest request)
  {
    var aiModel = configuration["AiModel"]!;
    var client = CreateAgentsClient();

    var agentResponse = await client.CreateAgentAsync(
      model: aiModel,
      name: request.Name,
      instructions: request.Instructions);

    return new Agent(
      agentResponse.Value.Id,
      agentResponse.Value.Name,
      agentResponse.Value.Instructions);
  }
  
  public async Task<Thread> CreateThreadAsync()
  {
    var client = CreateAgentsClient();

    var threadResponse = await client.CreateThreadAsync();

    return new Thread(threadResponse.Value.Id);
  }
  
  public async Task<Message> CreateRunAsync(CreateRunRequest request)
  { 
      var client = CreateAgentsClient();
      
      await client.CreateMessageAsync(
            request.ThreadId,
            MessageRole.User,
            request.Message);
              
      Response<ThreadRun> runResponse = await client.CreateRunAsync(
            request.ThreadId,
            request.AgentId,
            additionalInstructions: "");
        
    do
    {
      await Task.Delay(TimeSpan.FromMilliseconds(500));
      runResponse = await client.GetRunAsync(request.ThreadId, runResponse.Value.Id);
      }
      while (runResponse.Value.Status == RunStatus.Queued || runResponse.Value.Status == RunStatus.InProgress);

      Response<PageableList<ThreadMessage>> afterRunMessagesResponse = await client.GetMessagesAsync(request.ThreadId, order: ListSortOrder.Descending, limit: 1);

      var message = afterRunMessagesResponse.Value.Data.FirstOrDefault();

      if (message is null)
      {
        throw new Exception("No messages found after run.");
      }
      
      StringBuilder text = new();
  
      foreach (var contentItem in message.ContentItems)
      {
        if (contentItem is MessageTextContent textItem)
        {
          text.AppendLine(textItem.Text);
        }
      }
      
      if (message.ContentItems.Count == 1 && text.Length > 0)
      {
        text.Length--;
      }

      return new Message(message.Id, message.Role.ToString(), text.ToString());
  }
  public async Task<IEnumerable<Message>> ListMessagesAsync(string threadId)
    {
        var client = CreateAgentsClient();

        Response<PageableList<ThreadMessage>> messagesResponse = await client.GetMessagesAsync(threadId);

        return messagesResponse.Value.Data.Select(message =>
        {
            StringBuilder text = new();

            foreach (var contentItem in message.ContentItems)
            {
                if (contentItem is MessageTextContent textItem)
                {
                    text.AppendLine(textItem.Text);
                }
            }

            if (message.ContentItems.Count == 1 && text.Length > 0)
            {
                text.Length--;
            }

            return new Message(message.Id, message.Role.ToString(), text.ToString());
        });
    }
}