using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniERP.Web.Features.Positions;

namespace MiniERP.Web.Pages.Positions;

public sealed class IndexModel(IPositionQueries queries) : PageModel
{
    public IReadOnlyList<PositionSummary> Positions { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Positions = await queries.GetAllAsync(cancellationToken);
    }
}
