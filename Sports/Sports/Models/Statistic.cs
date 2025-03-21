using System.ComponentModel.DataAnnotations;

namespace Sports.Models
{
    public class Statistic
    {
        public int Id { get; set; }

        [Display(Name = "Team")]
        [Required]
        public int TeamId { get; set; }
        public Team? Team { get; set; }

        [Display(Name = "Competition")]
        [Required]
        public int CompetitionId { get; set; }
        public Competition? Competition { get; set; }
        public int Value { get; set; } = 0;

        [Display(Name = "Statistic Type")]
        public int Statistic_TypeId { get; set; }

        [Display(Name = "Statistic Type")]
        public Statistic_Type? StatisticType { get; set; }
        public Result? Result { get; set; }
    }
}