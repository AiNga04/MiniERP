using System.ComponentModel.DataAnnotations;

namespace MiniERP.Web.Pages.Departments;

public sealed class DepartmentInput
{
    [Display(Name = "Tên phòng ban")]
    [Required(ErrorMessage = "Vui lòng nhập tên phòng ban.")]
    [StringLength(30, ErrorMessage = "Tên phòng ban tối đa 30 ký tự.")]
    public string DepartmentName { get; set; } = string.Empty;
}
