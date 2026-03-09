using System.ComponentModel.DataAnnotations;

namespace SplitWise.Application.DTOs.Expenses
{
    public class CreateExpenseRequestDto
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public int PaidByGroupMemberId { get; set; }

        [Required]
        public DateOnly ExpenseDate { get; set; }

        [Required]
        public List<ExpenseShareRequestDto> Shares { get; set; }
    }
}
