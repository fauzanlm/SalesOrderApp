using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesOrderApp.Models
{
    [Table("SO_ORDER")]
    public class Order
    {
        [Key]
        [Column("SO_ORDER_ID")]
        public int Id { get; set; }

        [Required]
        [Column("ORDER_NO", TypeName = "varchar(20)")]
        public string SalesOrderNumber { get; set; } = string.Empty;

        [Required]
        [Column("ORDER_DATE")]
        public DateTime OrderDate { get; set; }

        [Column("ADDRESS", TypeName = "varchar(100)")]
        public string? Address { get; set; } = string.Empty;

        [Required]
        [Column("COM_CUSTOMER_ID")]
        public int CustomerId { get; set; }

        public Customer? Customer { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();
    }
}
