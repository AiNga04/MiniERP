namespace MiniERP.Web.Features.Positions;

public interface IPositionQueries
{
    Task<IReadOnlyList<PositionSummary>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PositionSummary?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}