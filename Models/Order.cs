using System.ComponentModel.DataAnnotations;
using StanaGO.Enums;

namespace StanaGO.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; } = null!;

        [Required]
        public string SellerId { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool SellerConfirmation { get; set; } = false;
        public bool CustomerConfirmation { get; set; } = false;

        public ProductStatus Status { get; set; } = ProductStatus.Available;


        public virtual User Customer { get; set; } = null!;
        public virtual User Seller { get; set; } = null!;

        public virtual Product Product { get; set; } = null!;




    }
}
