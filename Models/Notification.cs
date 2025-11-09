using StanaGO.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StanaGO.Models
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength (450)] 
        public string ReceiverId { get; set; } = string.Empty;

        [Required]
        [StringLength (500)] 
        public string Message { get; set; } = string.Empty;

        [Required]
        public bool IsSeen { get; set; } = false;

        [Required]
        public NotificationType NotificationType { get; set; } 

        [StringLength (255)]
        public string? Url { get; set; } 

        public DateTimeOffset Time { get; set; } = DateTimeOffset.UtcNow;

        [ForeignKey (nameof (ReceiverId))]
        public virtual User Receiver { get; set; } = null!;
    }
}