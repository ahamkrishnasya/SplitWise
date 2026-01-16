using SplitWise.Domain.Comman;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Domain.Entities
{
    public class ExpenseShare : BaseEntity
    {
        [Required]
        public int ExpenseId { get; set; }
        public int GroupMemberId { get; set; }
        public decimal Amount { get; set; }


    }
}
