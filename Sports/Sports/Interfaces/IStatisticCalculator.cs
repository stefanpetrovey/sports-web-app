using Sports.Models;

namespace Sports.Interfaces
{
    public interface IStatisticCalculator
    {
        // <summary>
        // This method calculates all of the statistics for team 1. For every 
        // sport class the statistics are different but the method is the same.
        // </summary>
        public void CalculateStatisticsTeam1(List<Result> team1Matches, List<Statistic> statisticsTeam1, Result result, double teamMatches, double teamMatchesWithWins, string sport);

        // <summary>
        // This method calculates all of the statistics for team 2. For every
        // sport class the statistics are different but the method is the same.
        // </summary>
        public void CalculateStatisticsTeam2(List<Result> team2Matches, List<Statistic> statisticsTeam2, Result result, double teamMatches, double teamMatchesWithWins, string sport);

        // <summary>
        // This method deletes all of the statistics for team 1. For every 
        // sport class the statistics are different but the method is the same.
        // </summary>
        public void DeleteStatisticsTeam1(List<Result> team1Matches, List<Statistic> statisticsTeam1, Result result, double teamMatches, double teamMatchesWithWins, string sport);

        // <summary>
        // This method deletes all of the statistics for team 2. For every 
        // sport class the statistics are different but the method is the same.
        // </summary>
        public void DeleteStatisticsTeam2(List<Result> team2Matches, List<Statistic> statisticsTeam2, Result result, double teamMatches, double teamMatchesWithWins, string sport);
    }
}