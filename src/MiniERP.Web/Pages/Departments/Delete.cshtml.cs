using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniERP.Web.Features.Departments;

namespace MiniERP.Web.Pages.Departments;

public sealed class DeleteModel(IDepartmentQueries queries, IDepartmentCommands commands) : PageModel
{
    public DepartmentSummary Department { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        DepartmentSummary? department = await queries.GetByIdAsync(id, cancellationToken);
        if (department is null) return NotFound();
        Department = department;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        DepartmentWriteResult result = await commands.DeleteAsync(id, cancellationToken);
        if (result.Status == DepartmentWriteStatus.NotFound) return NotFound();
        TempData["StatusMessage"] = "Đã xóa phòng ban.";
        return RedirectToPage("Index");
    }
}
