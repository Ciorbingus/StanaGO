using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StanaGO.Models
{
    public class PrivateMessage
    {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 

       

        [Required]
        [StringLength (450)] 
        public string SenderId { get; set; } = string.Empty;

        [Required]
        [StringLength (450)]
        public string ReceiverId { get; set; } = string.Empty;

        [Required]
        [StringLength (1000)] 
        public string Content { get; set; } = string.Empty;

        public DateTimeOffset SentTime { get; set; } = DateTimeOffset.UtcNow;

        [Required]
        public bool IsRead { get; set; } = false; 


        [ForeignKey (nameof (SenderId))]
        public virtual User Sender { get; set; } = null!;

        [ForeignKey (nameof (ReceiverId))]
        public virtual User Receiver { get; set; } = null!;
    }
}