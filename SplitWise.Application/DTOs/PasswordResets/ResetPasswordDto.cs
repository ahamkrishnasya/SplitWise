namespace SplitWise.Application.DTOs.PasswordResets
{
    public class ResetPasswordDto
    {
        public string? Token { get; set; }
        public string? NewPassword { get; set; }
    }
}
