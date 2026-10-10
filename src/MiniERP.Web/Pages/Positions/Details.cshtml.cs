using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniERP.Web.Features.Positions;

namespace MiniERP.Web.Pages.Positions;

public sealed class DetailsModel(IPositionQueries queries) : PageModel
{
    public PositionSummary Position { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        PositionSummary? position = await queries.GetByIdAsync(id, cancellationToken);
        if (position is null) return NotFound();
        Position = position;
        return Page();
    }
}
