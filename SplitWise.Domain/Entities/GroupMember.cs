using SplitWise.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplitWise.Domain.Common;
namespace SplitWise.Domain.Entities
{
    public class GroupMember : BaseEntity
    {
        [Required]
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public bool? IsAdmin { get; set; }
        public User User { get; set; }
        public virtual Groups Groups { get; set; }

    }
}
