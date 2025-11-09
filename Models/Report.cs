using StanaGO.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StanaGO.Models
{
    public class Report
    {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength (100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength (1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength (450)]
        public string ReporterId { get; set; } = string.Empty;

        [Required]
        public string ReportedId { get; set; } = string.Empty;

        public DateTimeOffset TimeSubmitted { get; set; } = DateTimeOffset.UtcNow;

        [Required]
        public ReportStatus Status { get; set; } = ReportStatus.Open;

        [StringLength (450)]
        public string? ModeratorId { get; set; } 

    
        [ForeignKey (nameof (ReporterId))]
        public virtual User Reporter { get; set; } = null!;

        [ForeignKey (nameof (ModeratorId))]
        public virtual Moderator? Moderator { get; set; }
    }
}