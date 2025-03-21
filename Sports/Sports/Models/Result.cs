using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sports.Models
{
    public class Result
    {
        public int Id { get; set; }

        [Display(Name = "Team 1")]
        public int Team1Id { get; set; }

        [ForeignKey("Team1Id")]
        [InverseProperty("Results1")]
        [Display(Name = "Team 1")]
        public Team? Team1 { get; set; }

        [Display(Name = "Team 2")]
        public int Team2Id { get; set; }

        [ForeignKey("Team2Id")]
        [InverseProperty("Results2")]
        [Display(Name = "Team 2")]
        public Team? Team2 { get; set; }
            
        [Display(Name = "Team 1 Result")]
        public int Team1Result { get; set; }

        [Display(Name = "Team 2 Result")]
        public int Team2Result { get; set; }
        public List<Statistic> Statistics { get; set; } = new List<Statistic>();
    }
}