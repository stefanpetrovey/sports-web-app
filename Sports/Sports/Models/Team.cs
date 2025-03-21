using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sports.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }

        [Display(Name = "Sport")]
        public int SportId { get; set; }
        public Sport? Sport { get; set; }

        [Display(Name = "Competition")]
        public int? CompetitionId { get; set; }
        public Competition? Competition { get; set; }
        public List<Statistic> Statistics { get; set; } = new List<Statistic>();

        [InverseProperty("Team1")]
        public ICollection<Result> Results1 { get; set; } = new List<Result>();

        [InverseProperty("Team2")]
        public ICollection<Result> Results2 { get; set; } = new List<Result>();
    }
}
