using SplitWise.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace SplitWise.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MobileNumber { get; set; }

        public ICollection<GroupMember> GroupMembers { get; set; }
            = new HashSet<GroupMember>();
    }
}