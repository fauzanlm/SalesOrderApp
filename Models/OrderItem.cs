using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesOrderApp.Models
{
    [Table("SO_ITEM")]
    public class OrderItem
    {
        [Key]
        [Column("SO_ITEM_ID")]
        public int Id { get; set; }

        [Required]
        [Column("SO_ORDER_ID")]
        public int OrderId { get; set; }

        [Required]
        [Column("ITEM_NAME", TypeName = "varchar(100)")]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [Column("QUANTITY")]
        public int Quantity { get; set; }

        [Required]
        [Column("PRICE",TypeName = "float")]
        public float Price { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float Total => Quantity * Price;

        public Order? Order { get; set; }
    }
}
