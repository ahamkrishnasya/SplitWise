//using System.ComponentModel.DataAnnotations;
//using SplitWise.Domain.Comman;

//namespace SplitWise.Domain.Entities
//{
//    public class User: BaseEntity
//    {
//        [Required]
//        [StringLength(100)]
//        public string? AccountName { get; set; }

//        [Required]
//        [StringLength(100)]
//        [RegularExpression( @"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
//        public string? Email { get; set; }

//        [Required]
//        public string? Password { get; set; }

//        [Required]
//        public string? FName { get; set; }

//        [Required]
//        public string? LName { get; set; }

//        [StringLength (15)]
//        public string? MobileNumber { get; set; }
//        public ICollection<GroupMember> GroupMembers { get; set; } = new HashSet<GroupMember>();
//    }
//}
