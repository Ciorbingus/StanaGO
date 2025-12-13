using StanaGO.Enums;
using System.ComponentModel.DataAnnotations;

namespace StanaGO.ViewModels
{
    public class ThreatViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Selectează tipul amenințării.")]
        public ThreatType Type { get; set; }

        [Display(Name = "Descriere / Detalii")]
        public string? Description { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Display(Name = "Poză")]
        public IFormFile? ImageFile { get; set; }

        public string? ExistingImagePath { get; set; }

    }
}