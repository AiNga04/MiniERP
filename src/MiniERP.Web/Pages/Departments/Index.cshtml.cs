using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniERP.Web.Domain.Entities;
using MiniERP.Web.Infrastructure.Persistence;

namespace MiniERP.Web.Pages.Departments;

public class IndexModel : PageModel
{
    private readonly AppDbContext _dbContext;

    public IndexModel(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Department> Departments { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Departments = await _dbContext.Departments
            .AsNoTracking()
            .OrderBy(x => x.DepartmentName)
            .ToListAsync();
    }
}