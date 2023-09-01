using System.Text.Json.Serialization;

namespace ERS.Models
{
    public class Expenselines
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int ExpenseId { get; set; }
        public virtual Expense? Expense { get; set; }
        public int ItemId { get; set; }
        [JsonIgnore]
        public virtual Item? Item { get; set;}

    }
}
