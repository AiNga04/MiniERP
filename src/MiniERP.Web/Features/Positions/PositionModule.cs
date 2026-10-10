namespace MiniERP.Web.Features.Positions;

internal static class PositionModule
{
    public static IServiceCollection AddPositions(this IServiceCollection services)
    {
        services.AddScoped<IPositionQueries, PositionQueries>();
        services.AddScoped<IPositionCommands, PositionCommands>();
        return services;
    }
}
z