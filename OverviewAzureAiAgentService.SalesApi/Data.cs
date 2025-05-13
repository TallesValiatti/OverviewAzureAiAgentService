namespace OverviewAzureAiAgentService.SalesApi;

public class Sales
{
    private static readonly DateTime StartDate = new DateTime(2025, 3, 10);
    private static readonly DateTime EndDate = new DateTime(2025, 3, 15);
    
    public static IEnumerable<Sale> Items => Enumerable.Range(1, 20).Select(index => new Sale
    {
        Id = Guid.NewGuid(),
        CustomerEmail = $"customer{index}@example.com",
        CreatedAt = StartDate.AddDays(Random.Shared.Next((EndDate - StartDate).Days + 1)),
        TotalValue = Math.Round(Random.Shared.NextDouble() * 1000, 2)
    }).ToList();
}