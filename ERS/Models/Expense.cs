using System.ComponentModel.DataAnnotations.Schema;

namespace ERS.Models
{
    public class Expense
    {
        public int Id { get; set; }
        [Column(TypeName ="decimal(11,2)")]
        public decimal Price { get; set; }

    }
}
