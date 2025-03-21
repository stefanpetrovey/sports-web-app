namespace Sports.Models
{
    public class Sport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Team> Teams { get; set; } = new List<Team>();
        public List<Competition> Competitions { get; set; } = new List<Competition> { };
    }
}
