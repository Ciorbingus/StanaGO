using StanaGO.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StanaGO.Models
{
    public class Threat
    {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength (450)] 
        public string ReporterId { get; set; } = string.Empty;

        [Required]
        public ThreatType Type { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public DateTimeOffset TimeReported { get; set; } = DateTimeOffset.UtcNow;

        [Required]
        public ThreatStatus Status { get; set; } = ThreatStatus.Active;

        [StringLength (255)]
        public string? MapIcon { get; set; }

        [ForeignKey (nameof (ReporterId))]
        public virtual User Reporter { get; set; } = null!;

        [StringLength(255)]
        public string? ImagePath { get; set; }

        [NotMapped]
        [Display(Name = "Încarcă Imagine")]
        public IFormFile? ImageFile { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}