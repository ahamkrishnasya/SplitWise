using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Application.DTOs.Groups
{
    public class CreateGroupDto
    {
        public string GroupName { get; set; }    
        public string Description { get; set; }
        public string CreatedByUserId { get; set; }
    }
}
