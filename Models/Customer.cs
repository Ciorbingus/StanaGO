namespace StanaGO.Models
{
    public class Customer : User
    {
        public Customer ( string username, string passwordHash, string email, string firstName, string lastName )
            : base (username, passwordHash, email, firstName, lastName)
        { }

        public Customer ( ) { }

        public string? Address { get; set; }
    }

}
