namespace SplitWise.Application.DTOs.Settlements
{
    public class SettlementResponseDto
    {
        public int Id { get; set; }
        public int ExpenseShareId { get; set; }
        public decimal Amount { get; set; }
        public DateOnly SettlementDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
