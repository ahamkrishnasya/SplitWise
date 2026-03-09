using System.ComponentModel.DataAnnotations;

namespace SplitWise.Application.DTOs.Expenses
{
    public class ExpenseShareRequestDto
    {
        [Required]
        public int GroupMemberId { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
