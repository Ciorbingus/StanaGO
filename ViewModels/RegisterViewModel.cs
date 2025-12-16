using System.ComponentModel.DataAnnotations;

namespace StanaGO.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength (50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength (50)]
        public string LastName { get; set; } = string.Empty;

        
        [Required]
        [StringLength (50, MinimumLength = 3, ErrorMessage = "Username-ul trebuie să aibă între 3 și 50 de caractere.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType (DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType (DataType.Password)]
        [Display (Name = "Confirm password")]
        [Compare ("Password", ErrorMessage = "Parola și confirmarea nu se potrivesc.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}