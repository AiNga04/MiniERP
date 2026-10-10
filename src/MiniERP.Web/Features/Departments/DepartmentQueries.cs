using Microsoft.EntityFrameworkCore;
using MiniERP.Web.Domain.Entities;
using MiniERP.Web.Infrastructure.Persistence;

namespace MiniERP.Web.Features.Departments;

internal sealed class DepartmentQueries(AppDbContext dbContext) : IDepartmentQueries
{
    public async Task<IReadOnlyList<DepartmentSummary>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Department>()
            .AsNoTracking()
            .OrderBy(department => department.DepartmentId)
            .Select(department => new DepartmentSummary(department.DepartmentId, department.DepartmentName))
            .ToListAsync(cancellationToken);
    }

    public Task<DepartmentSummary?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Departments.AsNoTracking()
            .Where(department => department.DepartmentId == id)
            .Select(department => new DepartmentSummary(department.DepartmentId, department.DepartmentName))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
