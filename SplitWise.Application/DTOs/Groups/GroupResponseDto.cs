using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Application.DTOs.Groups
{
    public class GroupResponseDto
    {
        [Required]
        public int GroupId { get; set; } 
        public string GroupName { get; set; }    
        public string Description { get; set; }
        public int CreatedByUserId { get; set; }
        public string? CreatedByFirstName { get; set; }
        public string? CreatedByLastName { get; set; }
        public string? CreatedByEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }
}
