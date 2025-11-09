using Microsoft.AspNetCore.Identity;
using StanaGO.Enums;
using System.ComponentModel.DataAnnotations;

namespace StanaGO.Models
{
    public abstract class User : IdentityUser
    {
     
        [Required]
        [StringLength (50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength (50)]
        public string LastName { get; set; } = string.Empty;

        public DateTimeOffset RegistrationTime { get; set; } = DateTimeOffset.UtcNow;

        [Required]
        public UserStatus Status { get; set; } = UserStatus.Online; 

        public double? Latitude { get; set; } = null;
        public double? Longitude { get; set; } = null;

        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification> ();

        public User ( ) { }

        public User ( string firstName, string lastName )
        {
            FirstName = firstName;
            LastName = lastName;
        }

    }
}