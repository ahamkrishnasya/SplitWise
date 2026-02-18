using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Application.DTOs.Groups
{
    public class GroupRequestDto
    {
        [Required]
        public string GroupName { get; set; }    
        public string? Description { get; set; }
    }
}
