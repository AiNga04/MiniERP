using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MiniERP.Web.Domain.Entities;
using MiniERP.Web.Infrastructure.Persistence;

namespace MiniERP.Web.Features.Departments;

internal sealed class DepartmentCommands(AppDbContext dbContext) : IDepartmentCommands
{
    public async Task<DepartmentWriteResult> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        string normalizedName = name?.Trim() ?? string.Empty;
        if (normalizedName.Length is < 1 or > 30)
            return new(DepartmentWriteStatus.InvalidName);

        if (await dbContext.Departments.AnyAsync(department => department.DepartmentName == normalizedName, cancellationToken))
            return new(DepartmentWriteStatus.DuplicateName);

        Department department = Department.Create(normalizedName);
        dbContext.Departments.Add(department);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return new(DepartmentWriteStatus.Success, department.DepartmentId);
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            // The unique index handles concurrent requests passing the precheck.
            dbContext.Entry(department).State = EntityState.Detached;
            return new(DepartmentWriteStatus.DuplicateName);
        }
    }

    public async Task<DepartmentWriteResult> UpdateAsync(int id, string name, CancellationToken cancellationToken = default)
    {
        string normalizedName = name?.Trim() ?? string.Empty;
        if (normalizedName.Length is < 1 or > 30)
            return new(DepartmentWriteStatus.InvalidName);

        if (!await dbContext.Departments.AnyAsync(department => department.DepartmentId == id, cancellationToken))
            return new(DepartmentWriteStatus.NotFound);

        if (await dbContext.Departments.AnyAsync(department => department.DepartmentId != id && department.DepartmentName == normalizedName, cancellationToken))
            return new(DepartmentWriteStatus.DuplicateName);

        try
        {
            // The query filter is part of the UPDATE, so a concurrent delete
            // cannot be undone by a stale edit form. Name edits use last-write-wins.
            int affected = await dbContext.Departments.Where(department => department.DepartmentId == id)
                .ExecuteUpdateAsync(setters => setters.SetProperty(department => department.DepartmentName, normalizedName), cancellationToken);
            return new(affected == 0 ? DepartmentWriteStatus.NotFound : DepartmentWriteStatus.Success, id);
        }
        catch (SqlException exception) when (exception.Number is 2601 or 2627)
        {
            return new(DepartmentWriteStatus.DuplicateName);
        }
    }

    public async Task<DepartmentWriteResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        DateTime deletedAt = DateTime.UtcNow;
        int affected = await dbContext.Departments.Where(department => department.DepartmentId == id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(department => department.DeletedAt, (DateTime?)deletedAt), cancellationToken);
        return new(affected == 0 ? DepartmentWriteStatus.NotFound : DepartmentWriteStatus.Success, id);
    }
}
