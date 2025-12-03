using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace StanaGO.Models
{
    public class Profile
    {

        [Key]
        [StringLength(450)]
        public string Id { get; set; } 
        public DateTime DateOfBirth { get; set; }
        public string AvatarUrl { get; set; }

        public string LocationText { get; set; }
        public User User { get; set; }
        public string Bio { get; set; } = string.Empty;


    }
}
