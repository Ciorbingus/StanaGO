using StanaGO.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StanaGO.Models
{
    public class Sheepfarm
    {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength (100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength (255)]
        public string Address { get; set; } = string.Empty; 

        [Required]
        [StringLength (450)] 
        public string OwnerId { get; set; } = string.Empty;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [StringLength (255)]
        public string? MapIcon { get; set; } 

        [ForeignKey (nameof (OwnerId))]
        public virtual Shepherd Owner { get; set; } = null!;
        public virtual ICollection<Product> Products { get; set; } = new List<Product> ();
    }
}