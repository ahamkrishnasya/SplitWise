using SplitWise.Domain.Common;

namespace SplitWise.Domain.Entities
{
    public class Settlement : BaseEntity
    {
        public int ExpenseShareId { get; set; }
        public decimal Amount { get; set; }
        public DateOnly SettlementDate { get; set; }

        public ExpenseShare ExpenseShare { get; set; }
    }
}
