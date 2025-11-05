namespace StanaGO.Models
{
    public class Shepherd : User
    {
        public Shepherd ( string username, string passwordHash, string email, string firstName, string lastName )
            : base (username, passwordHash, email, firstName, lastName)
        { }

        public Shepherd ( ) { }

        public string? Address { get; set; }
    }

}
