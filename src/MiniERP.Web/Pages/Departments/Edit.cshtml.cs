using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniERP.Web.Features.Departments;

namespace MiniERP.Web.Pages.Departments;

public sealed class EditModel(IDepartmentQueries queries, IDepartmentCommands commands) : PageModel
{
    [BindProperty]
    public DepartmentInput Input { get; set; } = new();
    public int DepartmentId { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        DepartmentSummary? department = await queries.GetByIdAsync(id, cancellationToken);
        if (department is null) return NotFound();
        DepartmentId = id;
        Input.DepartmentName = department.DepartmentName;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        DepartmentId = id;
        if (!ModelState.IsValid)
        {
            if (await queries.GetByIdAsync(id, cancellationToken) is null) return NotFound();
            return Page();
        }

        DepartmentWriteResult result = await commands.UpdateAsync(id, Input.DepartmentName, cancellationToken);
        if (result.Status == DepartmentWriteStatus.NotFound) return NotFound();
        if (result.Status != DepartmentWriteStatus.Success)
        {
            ModelState.AddModelError("Input.DepartmentName", result.ErrorMessage);
            return Page();
        }

        TempData["StatusMessage"] = "Đã cập nhật phòng ban.";
        return RedirectToPage("Details", new { id });
    }
}
