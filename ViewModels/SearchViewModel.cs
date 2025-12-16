using StanaGO.Models;

namespace StanaGO.ViewModels
{
    public class SearchViewModel
    {
        public string Query { get; set; } = string.Empty;
        public string Filter { get; set; } = "all"; 

        public List<Profile> Profiles { get; set; } = new List<Profile>();
        public List<Sheepfarm> Farms { get; set; } = new List<Sheepfarm>();
        public List<Product> Products { get; set; } = new List<Product>();
    }
}