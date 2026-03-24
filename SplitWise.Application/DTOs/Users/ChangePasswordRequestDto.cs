namespace SplitWise.Application.DTOs.Users
{
    public class ChangePasswordRequestDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
