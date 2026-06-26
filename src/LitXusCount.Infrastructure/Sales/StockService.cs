using LitXusCount.Application.Sales;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Sales;

internal sealed class StockService(ApplicationDbContext db) : IStockService
{
    public async Task UpdateStockAsync(long productId, decimal quantity, bool isAddition, CancellationToken ct = default)
    {
        var product = await db.Products.FirstOrDefaultAsync(x => x.Id == productId, ct)
            ?? throw new InvalidOperationException($"Product {productId} not found.");

        if (isAddition)
            product.StockQuantity += quantity;
        else
            product.StockQuantity -= quantity;

        product.ModifiedAt = DateTime.UtcNow;
    }
}
