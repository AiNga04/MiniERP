using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniERP.Web.Features.Positions;

namespace MiniERP.Web.Pages.Positions;

public sealed class EditModel(IPositionQueries queries, IPositionCommands commands) : PageModel
{
    [BindProperty]
    public PositionInput Input { get; set; } = new();
    public int PositionId { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        PositionSummary? position = await queries.GetByIdAsync(id, cancellationToken);
        if (position is null) return NotFound();
        PositionId = id;
        Input.PositionName = position.PositionName;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        PositionId = id;
        if (!ModelState.IsValid)
        {
            if (await queries.GetByIdAsync(id, cancellationToken) is null) return NotFound();
            return Page();
        }

        PositionWriteResult result = await commands.UpdateAsync(id, Input.PositionName, cancellationToken);
        if (result.Status == PositionWriteStatus.NotFound) return NotFound();
        if (result.Status != PositionWriteStatus.Success)
        {
            ModelState.AddModelError("Input.PositionName", result.ErrorMessage);
            return Page();
        }

        TempData["StatusMessage"] = "Đã cập nhật chức vụ.";
        return RedirectToPage("Details", new { id });
    }
}
