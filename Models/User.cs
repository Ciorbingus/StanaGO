using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StanaGO.Models
{
    public abstract class User
    {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength (50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength (256)] 
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength (100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength (50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength (50)]
        public string LastName { get; set; } = string.Empty;

        public double? Latitude { get; set; } = null;
        public double? Longitude { get; set; } = null;

        protected User ( ) { }

        protected User ( string username, string passwordHash, string email, string firstName, string lastName )
        {
            Username = username;
            PasswordHash = passwordHash;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
