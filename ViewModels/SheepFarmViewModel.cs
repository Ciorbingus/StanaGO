using System.ComponentModel.DataAnnotations;

namespace StanaGO.ViewModels
{
    public class SheepFarmViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        [Display(Name = "Nume Stână")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Adresa este obligatorie")]
        [Display(Name = "Adresă (Text)")]
        public string Address { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }
    }
}