using System.ComponentModel.DataAnnotations;

namespace StanaGO.ViewModels
{
    public class BecomeShepherdViewModel
    {
        [Required (ErrorMessage = "Numărul de telefon este obligatoriu.")]
        [Phone (ErrorMessage = "Formatul numărului de telefon nu este valid.")]
        [Display (Name = "Număr de telefon")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
