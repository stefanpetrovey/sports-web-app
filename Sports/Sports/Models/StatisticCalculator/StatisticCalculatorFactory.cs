using Sports.Interfaces;

namespace Sports.Models.StatisticCalculator
{
    public class StatisticCalculatorFactory
    {
        public IStatisticCalculator GetStatistic(String statistic)
        {
            if (statistic == null)
            {
                return null;
            }
            if (statistic.Equals("Soccer"))
            {
                return new Soccer();
            }
            if (statistic.Equals("Basketball"))
            {
                return new Basketball();
            }
            if (statistic.Equals("Handball"))
            {
                return new Handball();
            }
            if (statistic.Equals("Volleyball"))
            {
                return new Volleyball();
            }
            if (statistic.Equals("Hockey"))
            {
                return new Hockey();
            }
            return null;
        }
    }
}