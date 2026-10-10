namespace MiniERP.Web.Features.Departments;

public interface IDepartmentCommands
{
    Task<DepartmentWriteResult> CreateAsync(string name, CancellationToken cancellationToken = default);
    Task<DepartmentWriteResult> UpdateAsync(int id, string name, CancellationToken cancellationToken = default);
    Task<DepartmentWriteResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
