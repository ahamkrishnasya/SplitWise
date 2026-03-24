using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Application.DTOs.Settlements
{
    public class PatchSettlementRequestDto
    {
        public decimal? Amount { get; set; }

        public DateOnly? SettlementDate { get; set; }
    }
}

