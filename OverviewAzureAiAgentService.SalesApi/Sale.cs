namespace OverviewAzureAiAgentService.SalesApi;

public class Sale
{
    public Guid Id { get; set; }
    public string CustomerEmail { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public double TotalValue { get; set; }
}