using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MiniERP.Web.Domain.Entities;
using MiniERP.Web.Infrastructure.Persistence;

namespace MiniERP.Web.Features.Positions;

internal sealed class PositionCommands(AppDbContext dbContext) : IPositionCommands
{
    public async Task<PositionWriteResult> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        string normalizedName = name?.Trim() ?? string.Empty;
        if (normalizedName.Length is < 1 or > 30)
            return new(PositionWriteStatus.InvalidName);

        if (await dbContext.Positions.AnyAsync(position => position.PositionName == normalizedName, cancellationToken))
            return new(PositionWriteStatus.DuplicateName);

        Position position = Position.Create(normalizedName);
        dbContext.Positions.Add(position);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return new(PositionWriteStatus.Success, position.PositionId);
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            // The unique index handles concurrent requests passing the precheck.
            dbContext.Entry(position).State = EntityState.Detached;
            return new(PositionWriteStatus.DuplicateName);
        }
    }

    public async Task<PositionWriteResult> UpdateAsync(int id, string name, CancellationToken cancellationToken = default)
    {
        string normalizedName = name?.Trim() ?? string.Empty;
        if (normalizedName.Length is < 1 or > 30)
            return new(PositionWriteStatus.InvalidName);

        if (!await dbContext.Positions.AnyAsync(position => position.PositionId == id, cancellationToken))
            return new(PositionWriteStatus.NotFound);

        if (await dbContext.Positions.AnyAsync(position => position.PositionId != id && position.PositionName == normalizedName, cancellationToken))
            return new(PositionWriteStatus.DuplicateName);

        try
        {
            // The query filter is part of the UPDATE, so a concurrent delete
            // cannot be undone by a stale edit form. Name edits use last-write-wins.
            int affected = await dbContext.Positions.Where(position => position.PositionId == id)
                .ExecuteUpdateAsync(setters => setters.SetProperty(position => position.PositionName, normalizedName), cancellationToken);
            return new(affected == 0 ? PositionWriteStatus.NotFound : PositionWriteStatus.Success, id);
        }
        catch (SqlException exception) when (exception.Number is 2601 or 2627)
        {
            return new(PositionWriteStatus.DuplicateName);
        }
    }

    public async Task<PositionWriteResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        DateTime deletedAt = DateTime.UtcNow;
        int affected = await dbContext.Positions.Where(position => position.PositionId == id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(position => position.DeletedAt, (DateTime?)deletedAt), cancellationToken);
        return new(affected == 0 ? PositionWriteStatus.NotFound : PositionWriteStatus.Success, id);
    }
}
