using SplitWise.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Domain.Entities
{
    public class Groups: BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string? GroupName { get; set; }
        public string? Description { get; set; }
        public int CreatedByUserId { get; set; }

        public virtual ICollection<GroupMember>? GroupMembers { get; set; }
    }
}
