using System.ComponentModel.DataAnnotations;

namespace Sports.Models
{
    public class Competition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public List<Team> Teams { get; set; } = new List<Team>();
        public List<Statistic> Statistics { get; set; } = new List<Statistic>();

        [Display(Name = "Sport")]
        public int SportId { get; set; }
        public Sport? Sport { get; set; }
    }
}
