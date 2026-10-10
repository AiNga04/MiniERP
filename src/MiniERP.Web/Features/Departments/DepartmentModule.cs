namespace MiniERP.Web.Features.Departments;

internal static class DepartmentModule
{
    public static IServiceCollection AddDepartments(this IServiceCollection services)
    {
        services.AddScoped<IDepartmentQueries, DepartmentQueries>();
        services.AddScoped<IDepartmentCommands, DepartmentCommands>();
        return services;
    }
}
