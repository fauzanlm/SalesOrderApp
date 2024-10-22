using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesOrderApp.Models
{
    [Table("COM_CUSTOMER")]
    public class Customer
    {
        [Key]
        [Column("COM_CUSTOMER_ID")]
        public int Id { get; set; }

        [Column("CUSTOMER_NAME", TypeName = "varchar(100)")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Order>? Orders { get; set; }
    }
}
