using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Application.DTOs.Groups
{
    public class GroupMemberRequestDto
    {
        [Required]
        public int GroupId { get; set; }    
        public int[] MemberId { get; set; }
    }
}
