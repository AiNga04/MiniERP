using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniERP.Web.Features.Departments;

namespace MiniERP.Web.Pages.Departments;

public sealed class CreateModel(IDepartmentCommands commands) : PageModel
{
    [BindProperty]
    public DepartmentInput Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();
        DepartmentWriteResult result = await commands.CreateAsync(Input.DepartmentName, cancellationToken);
        if (result.Status != DepartmentWriteStatus.Success)
        {
            ModelState.AddModelError("Input.DepartmentName", result.ErrorMessage);
            return Page();
        }

        TempData["StatusMessage"] = "Đã tạo phòng ban.";
        return RedirectToPage("Details", new { id = result.DepartmentId });
    }
}
