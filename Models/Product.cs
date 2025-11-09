using StanaGO.Enums;
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

        [StringLength (500)]
        public string? Description { get; set; }

        [Required]
        [Column (TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [StringLength (255)]
        public string? ImagePath { get; set; }

        public DateTimeOffset TimePublished { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? TimeExpiration { get; set; } = null; 

        [Required]
        public ProductStatus Status { get; set; } = ProductStatus.Available;

        
        [Required]
        public int FarmId { get; set; }

        [ForeignKey (nameof (FarmId))]
        public virtual Sheepfarm Farm { get; set; } = null!; 

    }
}