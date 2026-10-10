namespace MiniERP.Web.Domain.Entities;

internal sealed class Position
{
    public int PositionId { get; private set; }
    public string PositionName { get; private set; } = string.Empty;
    public DateTime? DeletedAt { get; private set; }

    private Position() { }

    public static Position Create(string name)
    {
        string normalizedName = name.Trim();
        if (normalizedName.Length is < 1 or > 30)
        {
            throw new ArgumentException("Tên vị trí phải có từ 1 đến 30 ký tự.", nameof(name));
        }

        return new Position { PositionName = normalizedName };
    }
}