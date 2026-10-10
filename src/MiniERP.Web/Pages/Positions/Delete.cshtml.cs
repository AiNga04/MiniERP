using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniERP.Web.Features.Positions;

namespace MiniERP.Web.Pages.Positions;

public sealed class DeleteModel(IPositionQueries queries, IPositionCommands commands) : PageModel
{
    public PositionSummary Position { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        PositionSummary? position = await queries.GetByIdAsync(id, cancellationToken);
        if (position is null) return NotFound();
        Position = position;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        PositionWriteResult result = await commands.DeleteAsync(id, cancellationToken);
        if (result.Status == PositionWriteStatus.NotFound) return NotFound();
        TempData["StatusMessage"] = "Đã xóa chức vụ.";
        return RedirectToPage("Index");
    }
}
