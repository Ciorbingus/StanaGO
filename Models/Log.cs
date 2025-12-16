using StanaGO.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StanaGO.Models
{
    public class Log
    {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength (500)] 
        public string Description { get; set; } = string.Empty;

        [Required]
        public LogType LogType { get; set; } 

        public DateTimeOffset Time { get; set; } = DateTimeOffset.UtcNow;
    }
}