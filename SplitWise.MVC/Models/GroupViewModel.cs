namespace SplitWise.MVC.Models
{
    public class GroupViewModel
    {
        public string? GroupName { get; set; }
        public string? Description { get; set; }
        public string? CreatedByUserId { get; set; }

        public int GroupId { get; set; }
        public string UserId { get; set; }
        public bool? IsAdmin { get; set; }

        public int[] Members { get; set; }
    }
}
