using Microsoft.EntityFrameworkCore;
using MiniERP.Web.Domain.Entities;
using MiniERP.Web.Infrastructure.Persistence;

namespace MiniERP.Web.Features.Positions;

internal sealed class PositionQueries(AppDbContext dbContext) : IPositionQueries
{
	public async Task<IReadOnlyList<PositionSummary>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await dbContext.Set<Position>()
			.AsNoTracking()
			.OrderBy(position => position.PositionId)
			.Select(position => new PositionSummary(position.PositionId, position.PositionName))
			.ToListAsync(cancellationToken);
	}

	public Task<PositionSummary?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
	{
		return dbContext.Positions.AsNoTracking()
			.Where(position => position.PositionId == id)
			.Select(position => new PositionSummary(position.PositionId, position.PositionName))
			.SingleOrDefaultAsync(cancellationToken);
	}
}
