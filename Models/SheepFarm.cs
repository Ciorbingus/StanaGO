using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StanaGO.Models
{
    public class Sheepfarm
    {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength (100)]
        public string Name { get; set; } = string.Empty;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [ForeignKey ("Owner")]
        public int OwnerId { get; set; }
        public Shepherd Owner { get; set; }

        public Sheepfarm ( ) { }

        public Sheepfarm ( string name, double latitude, double longitude, Shepherd owner )
        {
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            Owner = owner;
            OwnerId = owner.Id;
        }
    }
}
