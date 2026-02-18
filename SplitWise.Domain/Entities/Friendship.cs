using SplitWise.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Domain.Entities
{
    public class Friendship : BaseEntity
    {
        public int UserId1 { get; set; }
        public User User1 { get; set; }
        public int UserId2 { get; set; }
        public User User2 { get; set; }
    }
}
