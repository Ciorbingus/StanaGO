using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StanaGO.Models
{
    public class Product
    {
        [Key] 
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength (100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength (500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range (0, double.MaxValue)]
        public decimal Price { get; set; }

        [StringLength (200)]
        public string? ImagePath { get; set; } = null;

        public Product ( ) { }

        public Product ( string name, string description, decimal price, string? imagePath = null )
        {
            Name = name;
            Description = description;
            Price = price;
            ImagePath = imagePath;
        }
    }
}
