namespace StanaGO.ViewModels
{
    public class ProfileViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string AvatarUrl { get; set; }
        public string LocationText { get; set; }
        public string UserId { get; set; }

       public double? Latitude { get; set; }
       public double? Longitude { get; set; }
       
       public string Bio { get; set; }

       public DateTime DateOfBirth { get; set; }


    }
}
