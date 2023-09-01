using ERS.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERS.Data {
    public class Expense {
        public int ID { get; set; }
        [StringLength(80)] public string Description { get; set; }
        [StringLength(10)] public string Status { get; set; } = "NEW";
        [Column(TypeName = "decimal(11,2)")]public decimal Total { get; set; } = 0;
        public int EmployeeID { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}
