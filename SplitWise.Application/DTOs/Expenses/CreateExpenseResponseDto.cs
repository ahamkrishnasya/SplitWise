namespace SplitWise.Application.DTOs.Expenses
{
    public class CreateExpenseResponseDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Description { get; set; }
        public decimal TotalAmount { get; set; }
        public int PaidByGroupMemberId { get; set; }
        public DateOnly ExpenseDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ExpenseShareResponseDto> Shares { get; set; }
    }
}
