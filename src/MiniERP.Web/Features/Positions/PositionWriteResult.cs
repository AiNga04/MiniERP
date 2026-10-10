namespace MiniERP.Web.Features.Positions;

public enum PositionWriteStatus { Success, NotFound, DuplicateName, InvalidName }

public sealed record PositionWriteResult(PositionWriteStatus Status, int? PositionId = null)
{
    public string ErrorMessage => Status switch
    {
        PositionWriteStatus.DuplicateName => "Tên vị trí đã tồn tại.",
        PositionWriteStatus.InvalidName => "Tên vị trí phải có từ 1 đến 30 ký tự.",
        PositionWriteStatus.NotFound => "Vị trí không tồn tại hoặc đã bị xóa.",
        _ => string.Empty
    };
}