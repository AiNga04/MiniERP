namespace MiniERP.Web.Features.Departments;

public interface IDepartmentQueries
{
    Task<IReadOnlyList<DepartmentSummary>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DepartmentSummary?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
