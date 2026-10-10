using System.ComponentModel.DataAnnotations;

namespace MiniERP.Web.Pages.Positions;

public sealed class PositionInput
{
    [Display(Name = "Tên chức vụ")]
    [Required(ErrorMessage = "Vui lòng nhập tên chức vụ.")]
    [StringLength(30, ErrorMessage = "Tên chức vụ tối đa 30 ký tự.")]
    public string PositionName { get; set; } = string.Empty;
}
