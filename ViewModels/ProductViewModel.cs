using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using StanaGO.Enums;

namespace StanaGO.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele produsului este obligatoriu")]
        [Display(Name = "Nume Produs")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Preț (RON)")]
        public decimal Price { get; set; }

        [Display(Name = "Descriere")]
        public string? Description { get; set; }

        [Display(Name = "Status")]
        public ProductStatus Status { get; set; } 


        [Display(Name = "Schimbă Imaginea")]
        public IFormFile? NewImage { get; set; }

        public string? CurrentImagePath { get; set; }
    }
}