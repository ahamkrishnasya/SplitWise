using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Application.DTOs.Settlements
{
    public class PatchSettlementRequestDto : IValidatableObject
    {
        public decimal? Amount { get; set; }

        public DateOnly? SettlementDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Amount is null && SettlementDate is null)
                yield return new ValidationResult(
                    "At least one of Amount or SettlementDate must be provided.",
                    new[] { nameof(Amount), nameof(SettlementDate) }
                );
        }
    }
}
