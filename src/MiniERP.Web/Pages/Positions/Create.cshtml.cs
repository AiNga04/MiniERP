using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniERP.Web.Features.Positions;

namespace MiniERP.Web.Pages.Positions;

public sealed class CreateModel(IPositionCommands commands) : PageModel
{
    [BindProperty]
    public PositionInput Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();
        PositionWriteResult result = await commands.CreateAsync(Input.PositionName, cancellationToken);
        if (result.Status != PositionWriteStatus.Success)
        {
            ModelState.AddModelError("Input.PositionName", result.ErrorMessage);
            return Page();
        }

        TempData["StatusMessage"] = "Đã tạo chức vụ.";
        return RedirectToPage("Details", new { id = result.PositionId });
    }
}
