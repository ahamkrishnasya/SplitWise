using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Application.DTOs.Friendships
{
    public class FriendshipRequestDto
    {
        [Required]
        public int[] FriendUserId { get; set; }   
    }
}
