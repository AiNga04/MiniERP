namespace MiniERP.Web.Features.Departments;

public enum DepartmentWriteStatus { Success, NotFound, DuplicateName, InvalidName }

public sealed record DepartmentWriteResult(DepartmentWriteStatus Status, int? DepartmentId = null)
{
    public string ErrorMessage => Status switch
    {
        DepartmentWriteStatus.DuplicateName => "Tên phòng ban đã tồn tại.",
        DepartmentWriteStatus.InvalidName => "Tên phòng ban phải có từ 1 đến 30 ký tự.",
        DepartmentWriteStatus.NotFound => "Phòng ban không tồn tại hoặc đã bị xóa.",
        _ => string.Empty
    };
}
