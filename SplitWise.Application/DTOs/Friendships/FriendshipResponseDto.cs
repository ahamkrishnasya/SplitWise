using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Application.DTOs.Friendships
{
    public class FriendshipResponseDto
    {
        public int Id { get; set; } 
        public int FriendUserId { get; set; }   
        public int CreatedByUserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
