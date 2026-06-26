namespace LitXusCount.Application.Sales;

public interface IStockService
{
    Task UpdateStockAsync(long productId, decimal quantity, bool isAddition, CancellationToken ct = default);
}
