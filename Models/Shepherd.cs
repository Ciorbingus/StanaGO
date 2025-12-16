namespace StanaGO.Models
{
    public class Shepherd : User
    {
        public virtual ICollection<Sheepfarm> Farms { get; set; } = new List<Sheepfarm> ();

        public Shepherd ( ) : base () { }

        public Shepherd ( string firstName, string lastName ) : base (firstName, lastName) { }

    }
}