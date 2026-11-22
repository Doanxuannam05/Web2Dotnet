using System.ComponentModel.DataAnnotations;

namespace DemoAPINET.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Tên tài khoản không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên tài khoản tối đa 100 ký tự.")]
        public string UserName { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Họ và tên tối đa 100 ký tự.")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [MaxLength(150, ErrorMessage = "Email tối đa 150 ký tự.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống.")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // Role có thể để trống, hệ thống sẽ tự động gán dựa theo đuôi .nv, .hr, .ad
        public string? Role { get; set; }
    }
}
