using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace StanaGO.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Prenumele este obligatoriu.")]
        [StringLength(50, ErrorMessage = "Prenumele nu poate depăși 50 de caractere.")]
        [Display(Name = "Prenume")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Numele este obligatoriu.")]
        [StringLength(50, ErrorMessage = "Numele nu poate depăși 50 de caractere.")]
        [Display(Name = "Nume")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Descrierea este prea lungă.")]
        [Display(Name = "Despre mine (Bio)")]
        public string? Bio { get; set; }

        [Display(Name = "Număr de Telefon")]
        [Phone]
        public string? PhoneNumber { get; set; }

        [StringLength(200, ErrorMessage = "Adresa este prea lungă.")]
        [Display(Name = "Adresă")]
        public string? Address { get; set; }


        [Display(Name = "Schimbă Poza de Profil")]
        public IFormFile? ProfileImage { get; set; }

        public string? CurrentProfilePicture { get; set; }
    }
}