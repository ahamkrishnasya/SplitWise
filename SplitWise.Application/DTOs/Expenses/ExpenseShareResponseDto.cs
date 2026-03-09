namespace SplitWise.Application.DTOs.Expenses
{
    public class ExpenseShareResponseDto
    {
        public int Id { get; set; }
        public int GroupMemberId { get; set; }
        public decimal Amount { get; set; }
    }
}
