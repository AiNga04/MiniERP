namespace MiniERP.Web.Features.Positions;

public interface IPositionCommands
{
    Task<PositionWriteResult> CreateAsync(string name, CancellationToken cancellationToken = default);
    Task<PositionWriteResult> UpdateAsync(int id, string name, CancellationToken cancellationToken = default);
    Task<PositionWriteResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
