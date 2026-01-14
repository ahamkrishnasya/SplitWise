using SplitWise.Domain.Comman;
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
        public int PaidByUserId { get; set; }
        public int PaidToUserId { get; set; }
        public decimal Amount { get; set; }
    }
}
