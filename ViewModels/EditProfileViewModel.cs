using System.ComponentModel.DataAnnotations;

namespace StanaGO.ViewModels
{
    public class EditProfileViewModel
    {
        public string UserId { get; set; }
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(500)]
        public string Bio { get; set; }

        [StringLength(255)]
        public string LocationText { get; set; }

       
        public string AvatarUrl { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }


    }
}
