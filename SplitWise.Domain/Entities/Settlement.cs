using SplitWise.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Domain.Entities
{
    public class Settlement : BaseEntity
    {
        public int GroupId { get; set; }
        public int PaidByGroupMemberId { get; set; }
        public int PaidToGroupMemberId { get; set; }
        public decimal Amount { get; set; }
    }
}
