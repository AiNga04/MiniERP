namespace MiniERP.Web.Domain.Entities;

internal sealed class Department
{
    public int DepartmentId { get; private set; }
    public string DepartmentName { get; private set; } = string.Empty;
    public DateTime? DeletedAt { get; private set; }

    private Department() { }

    public static Department Create(string name)
    {
        string normalizedName = name.Trim();
        if (normalizedName.Length is < 1 or > 30)
        {
            throw new ArgumentException("Tên phòng ban phải có từ 1 đến 30 ký tự.", nameof(name));
        }

        return new Department { DepartmentName = normalizedName };
    }
}
