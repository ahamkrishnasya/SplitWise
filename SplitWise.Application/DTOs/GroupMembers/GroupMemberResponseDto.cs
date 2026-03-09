using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Application.DTOs.Groups
{
    public class GroupMemberResponseDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }   
        public int MemberId { get; set; }   
        public int CreatedByUserId { get; set; }
        public bool? IsAdmin { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
