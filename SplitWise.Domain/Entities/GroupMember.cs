using SplitWise.Domain.Comman;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Domain.Entities
{
    public class GroupMember : BaseEntity
    {
        [Required]
        public int GroupId { get; set; }
        public string UserId { get; set; }
        public bool? IsAdmin { get; set; }

        //public ApplicationUser User { get; set; }
        public virtual Groups Groups { get; set; }

    }
}
