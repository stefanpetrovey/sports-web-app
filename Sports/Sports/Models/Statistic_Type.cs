using System.ComponentModel.DataAnnotations;

namespace Sports.Models
{
    public class Statistic_Type
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Display(Name = "Order Of Appearence")]
        public int Order_Of_Appearence { get; set; }
        public List<Statistic> Statistic { get; set; } = new List<Statistic>();
    }
}
