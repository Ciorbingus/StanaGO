using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StanaGO.Models
{
    public class Profile
    {
        [Key]
        [ForeignKey("User")] 
        public string Id { get; set; }

        public virtual User User { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(255)]
        public string? ImagePath { get; set; }

        [NotMapped]
        [Display(Name = "Încarcă Imagine de Profil")]
        public IFormFile? ImageFile { get; set; }
    }
}