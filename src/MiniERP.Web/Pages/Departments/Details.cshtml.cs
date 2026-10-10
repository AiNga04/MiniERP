using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniERP.Web.Features.Departments;

namespace MiniERP.Web.Pages.Departments;

public sealed class DetailsModel(IDepartmentQueries queries) : PageModel
{
    public DepartmentSummary Department { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        DepartmentSummary? department = await queries.GetByIdAsync(id, cancellationToken);
        if (department is null) return NotFound();
        Department = department;
        return Page();
    }
}
