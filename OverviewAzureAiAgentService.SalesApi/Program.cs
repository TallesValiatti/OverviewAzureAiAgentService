using OverviewAzureAiAgentService.SalesApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.UseHttpsRedirection();

app.MapGet("/sales", (DateTime? startDate, DateTime? endDate) =>
{
    var filteredSales = Sales.Items;
    
    if(startDate.HasValue)
    {
        filteredSales = filteredSales.Where(sale => sale.CreatedAt >= startDate.Value);
    }
    
    if(endDate.HasValue)
    {
        filteredSales = filteredSales.Where(sale => sale.CreatedAt <= endDate.Value);
    }
    
    return filteredSales;
})
.WithName("GetSales");

app.Run();

