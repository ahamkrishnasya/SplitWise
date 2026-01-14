using SplitWise.Domain.Comman;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Domain.Entities
{
    public class Expense : BaseEntity
    {
        [Required]
        public int GroupId { get; set; }
        public string PaidByUserId { get; set; }
        public string Description { get; set; }
        public decimal TotalAmount { get; set; }

    }
}
