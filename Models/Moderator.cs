namespace StanaGO.Models
{
    public class Moderator : User
    {
        public virtual ICollection<Report> ResolvedReports { get; set; } = new List<Report> ();

        public int ReportsProcessedCount { get; set; } = 0;

    }
}