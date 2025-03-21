using Sports.Models;
using System.Diagnostics.CodeAnalysis;

namespace UnitTests
{
    [TestClass]
    public class isSport
    {
        [TestMethod]
        public void IdValid()
        {
            // arrange
            Sport sport = new Sport();
            sport.Id = 1;

            // act
            int result = sport.Id;

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void NameValid()
        {
            // arrange
            Sport sport = new Sport();
            sport.Name = "Soccer";

            // act
            string result = sport.Name;

            // assert
            Assert.AreEqual("Soccer", result);
        }

        [TestMethod]
        public void TeamsValid()
        {
            // arrange
            Sport sport = new Sport();
            sport.Teams = new List<Team>
            {
                new Team { Id = 1, Name = "Real Madrid"},
                new Team { Id = 2, Name = "Barcelona"}
            };

            var expectedTeams = new List<Team>
            {
                new Team { Id = 1, Name = "Real Madrid"},
                new Team { Id = 2, Name = "Barcelona"}
            };

            // act
            var result = sport.Teams;

            // assert
            for (int i = 0; i < expectedTeams.Count; i++)
            {
                Assert.AreEqual(expectedTeams[i].Id, result[i].Id);
                Assert.AreEqual(expectedTeams[i].Name, result[i].Name);
            }
        }

        [TestMethod]
        public void CompetitionsValid()
        {
            // arrange
            Sport sport = new Sport();
            sport.Competitions = new List<Competition>
            {
                new Competition { Id = 1, Name = "Premier League"},
                new Competition { Id = 2, Name = "LaLiga"}
            };

            var expectedCompetitions = new List<Competition>
            {
               new Competition { Id = 1, Name = "Premier League" },
               new Competition { Id = 2, Name = "LaLiga"}
            };

            // act
            var result = sport.Competitions;

            // assert
            for (int i = 0; i < expectedCompetitions.Count; i++)
            {
                Assert.AreEqual(expectedCompetitions[i].Id, result[i].Id);
                Assert.AreEqual(expectedCompetitions[i].Name, result[i].Name);
            }
        }
    }

    [TestClass]
    public class isCompetition
    {
        [TestMethod]
        public void IdValid()
        {
            // arrange
            Competition competition = new Competition();
            competition.Id = 1;

            // act
            int result = competition.Id;

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void NameValid()
        {
            // arrange
            Competition competition = new Competition();
            competition.Name = "LaLiga";

            // act
            string result = competition.Name;

            // assert
            Assert.AreEqual("LaLiga", result);
        }

        [TestMethod]
        public void CountryValid()
        {
            // arrange
            Competition competition = new Competition();
            competition.Country = "Spain";

            // act
            string result = competition.Country;

            // assert
            Assert.AreEqual("Spain", result);
        }

        [TestMethod]
        public void TeamsValid()
        {
            // arrange
            Competition competition = new Competition();
            competition.Teams = new List<Team>
            {
                new Team { Id = 1, Name = "Real Madrid"},
                new Team { Id = 2, Name = "Barcelona"}
            };

            var expectedTeams = new List<Team>
            {
                new Team { Id = 1, Name = "Real Madrid"},
                new Team { Id = 2, Name = "Barcelona"}
            };

            // act
            var result = competition.Teams;

            // assert
            for (int i = 0; i < expectedTeams.Count; i++)
            {
                Assert.AreEqual(expectedTeams[i].Id, result[i].Id);
                Assert.AreEqual(expectedTeams[i].Name, result[i].Name);
            }
        }

        [TestMethod]
        public void StatisticsValid()
        {
            // arrange
            Competition competition = new Competition();
            competition.Statistics = new List<Statistic>
            {
                new Statistic { Id = 1, TeamId = 1},
                new Statistic { Id = 2, TeamId = 2}
            };

            var expectedStatistics = new List<Statistic>
            {
                new Statistic { Id = 1, TeamId = 1},
                new Statistic { Id = 2, TeamId = 2}
            };

            // act
            var result = competition.Statistics;

            // assert
            for (int i = 0; i < expectedStatistics.Count; i++)
            {
                Assert.AreEqual(expectedStatistics[i].Id, result[i].Id);
                Assert.AreEqual(expectedStatistics[i].TeamId, result[i].TeamId);
            }
        }

        [TestMethod]
        public void SportIdValid()
        {
            // arrange
            Competition competition = new Competition();
            competition.SportId = 1;

            // act
            int result = competition.SportId;

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void SportValid()
        {
            // arrange
            Competition competition = new Competition();
            competition.Sport = new Sport() { Id = 1, Name = "Soccer" };

            Sport expectedSport = new Sport() { Id = 1, Name = "Soccer" };

            // act
            var result = competition.Sport;

            // assert
            Assert.AreEqual(expectedSport.Id, result.Id);
            Assert.AreEqual(expectedSport.Name, result.Name);
        }
    }

    [TestClass]
    public class isResult
    {
        [TestMethod]
        public void IdValid()
        {
            // arrange
            Result result = new Result();
            result.Id = 1;

            // act
            int resultUnitTest = result.Id;

            // assert
            Assert.AreEqual(1, resultUnitTest);
        }

        [TestMethod]
        public void Team1IdValid()
        {
            // arrange
            Result result = new Result();
            result.Team1Id = 1;

            // act
            int resultUnitTest = result.Team1Id;

            // assert
            Assert.AreEqual(1, resultUnitTest);
        }

        [TestMethod]
        public void Team1Valid()
        {
            // arrange
            Result result = new Result();
            result.Team1 = new Team() { Id = 1, Name = "Barcelona" };

            Team expectedTeam = new Team() { Id = 1, Name = "Barcelona" };

            // act
            var resultUnitTest = result.Team1;

            // assert
            Assert.AreEqual(expectedTeam.Id, resultUnitTest.Id);
            Assert.AreEqual(expectedTeam.Name, resultUnitTest.Name);
        }

        [TestMethod]
        public void Team2IdValid()
        {
            // arrange
            Result result = new Result();
            result.Team2Id = 1;

            // act
            int resultUnitTest = result.Team2Id;

            // assert
            Assert.AreEqual(1, resultUnitTest);
        }

        [TestMethod]
        public void Team2Valid()
        {
            // arrange
            Result result = new Result();
            result.Team2 = new Team() { Id = 1, Name = "Barcelona" };

            Team expectedTeam = new Team() { Id = 1, Name = "Barcelona" };

            // act
            var resultUnitTest = result.Team2;

            // assert
            Assert.AreEqual(expectedTeam.Id, resultUnitTest.Id);
            Assert.AreEqual(expectedTeam.Name, resultUnitTest.Name);
        }

        [TestMethod]
        public void Team1ResultValid()
        {
            // arrange
            Result result = new Result();
            result.Team1Result = 1;

            // act
            int resultUnitTest = result.Team1Result;

            // assert
            Assert.AreEqual(1, resultUnitTest);
        }

        [TestMethod]
        public void Team2ResultValid()
        {
            // arrange
            Result result = new Result();
            result.Team2Result = 1;

            // act
            int resultUnitTest = result.Team2Result;

            // assert
            Assert.AreEqual(1, resultUnitTest);
        }

        [TestMethod]
        public void StatisticsValid()
        {
            // arrange
            Result result = new Result();
            result.Statistics = new List<Statistic>
            {
                new Statistic { Id = 1, TeamId = 1},
                new Statistic { Id = 2, TeamId = 2}
            };

            var expectedStatistics = new List<Statistic>
            {
                new Statistic { Id = 1, TeamId = 1},
                new Statistic { Id = 2, TeamId = 2}
            };

            // act
            var resultUnitTest = result.Statistics;

            // assert
            for (int i = 0; i < expectedStatistics.Count; i++)
            {
                Assert.AreEqual(expectedStatistics[i].Id, resultUnitTest[i].Id);
                Assert.AreEqual(expectedStatistics[i].TeamId, resultUnitTest[i].TeamId);
            }
        }
    }

    [TestClass]
    public class isStatistic
    {
        [TestMethod]
        public void IdValid()
        {
            // arrange
            Statistic statistic = new Statistic();
            statistic.Id = 1;

            // act
            int result = statistic.Id;

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TeamIdValid()
        {
            // arrange
            Statistic statistic = new Statistic();
            statistic.TeamId = 1;

            // act
            int result = statistic.TeamId;

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TeamValid()
        {
            // arrange
            Statistic statistic = new Statistic();
            statistic.Team = new Team() { Id = 1, Name = "Barcelona" };

            Team expectedTeam = new Team() { Id = 1, Name = "Barcelona" };

            // act
            var result = statistic.Team;

            // assert
            Assert.AreEqual(expectedTeam.Id, result.Id);
            Assert.AreEqual(expectedTeam.Name, result.Name);
        }

        [TestMethod]
        public void CompetitionIdValid()
        {
            // arrange
            Statistic statistic = new Statistic();
            statistic.CompetitionId = 1;

            // act
            int result = statistic.CompetitionId;

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void CompetitionValid()
        {
            // arrange
            Statistic statistic = new Statistic();
            statistic.Competition = new Competition() { Id = 1, Name = "LaLiga" };

            Competition expectedCompetition = new Competition() { Id = 1, Name = "LaLiga" };

            // act
            var result = statistic.Competition;

            // assert
            Assert.AreEqual(expectedCompetition.Id, result.Id);
            Assert.AreEqual(expectedCompetition.Name, result.Name);
        }

        [TestMethod]
        public void ValueValid()
        {
            // arrange
            Statistic statistic = new Statistic();
            statistic.Value = 1;

            // act
            int result = statistic.Value;

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Statistic_TypeIdValid()
        {
            // arrange
            Statistic statistic = new Statistic();
            statistic.Statistic_TypeId = 1;

            // act
            int result = statistic.Statistic_TypeId;

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void StatisticTypeValid()
        {
            // arrange
            Statistic statistic = new Statistic();
            statistic.StatisticType = new Statistic_Type() { Id = 1, Name = "Points" };

            Competition expectedStatistic_Type = new Competition() { Id = 1, Name = "Points" };

            // act
            var result = statistic.StatisticType;

            // assert
            Assert.AreEqual(expectedStatistic_Type.Id, result.Id);
            Assert.AreEqual(expectedStatistic_Type.Name, result.Name);
        }

        [TestMethod]
        public void ResultValid()
        {
            // arrange
            Statistic statistic = new Statistic();
            statistic.Result = new Result() { Id = 1, Team1Id = 1, Team2Id = 1 };

            Result expectedResult = new Result() { Id = 1, Team1Id = 1, Team2Id = 1 };

            // act
            var result = statistic.Result;

            // assert
            Assert.AreEqual(expectedResult.Id, result.Id);
            Assert.AreEqual(expectedResult.Team1Id, result.Team1Id);
            Assert.AreEqual(expectedResult.Team2Id, result.Team2Id);
        }
    }

    [TestClass]
    public class isStatistic_Type
    {
        [TestMethod]
        public void IdValid()
        {
            // arrange
            Statistic_Type statisticType = new Statistic_Type();
            statisticType.Id = 1;

            // act
            int result = statisticType.Id;

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void NameValid()
        {
            // arrange
            Statistic_Type statisticType = new Statistic_Type();
            statisticType.Name = "Points";

            // act
            string result = statisticType.Name;

            // assert
            Assert.AreEqual("Points", result);
        }

        [TestMethod]
        public void Order_Of_AppearenceValid()
        {
            // arrange
            Statistic_Type statisticType = new Statistic_Type();
            statisticType.Order_Of_Appearence = 1;

            // act
            int result = statisticType.Order_Of_Appearence;

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void StatisticValid()
        {
            // arrange
            Statistic_Type statisticType = new Statistic_Type();
            statisticType.Statistic = new List<Statistic>
            {
                new Statistic { Id = 1, TeamId = 1},
                new Statistic { Id = 2, TeamId = 2}
            };

            var expectedStatistics = new List<Statistic>
            {
                new Statistic { Id = 1, TeamId = 1},
                new Statistic { Id = 2, TeamId = 2}
            };

            // act
            var result = statisticType.Statistic;

            // assert
            for (int i = 0; i < expectedStatistics.Count; i++)
            {
                Assert.AreEqual(expectedStatistics[i].Id, result[i].Id);
                Assert.AreEqual(expectedStatistics[i].TeamId, result[i].TeamId);
            }
        }
    }

    [TestClass]
    public class isTeam
    {
        [TestMethod]
        public void IdValid()
        {
            // arrange
            Team team = new Team();
            team.Id = 1;

            // act
            int result = team.Id;

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void NameValid()
        {
            // arrange
            Team team = new Team();
            team.Name = "Real Madrid";

            // act
            string result = team.Name;

            // assert
            Assert.AreEqual("Real Madrid", result);
        }

        [TestMethod]
        public void CountryValid()
        {
            // arrange
            Team team = new Team();
            team.Country = "Spain";

            // act
            string result = team.Country;

            // assert
            Assert.AreEqual("Spain", result);
        }

        [TestMethod]
        public void SportIdValid()
        {
            // arrange
            Team team = new Team();
            team.SportId = 1;

            // act
            int result = team.SportId;

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void SportValid()
        {
            // arrange
            Team team = new Team();
            team.Sport = new Sport() { Id = 1, Name = "Soccer" };

            Sport expectedSport = new Sport() { Id = 1, Name = "Soccer" };

            // act
            var result = team.Sport;

            // assert
            Assert.AreEqual(expectedSport.Id, result.Id);
            Assert.AreEqual(expectedSport.Name, result.Name);
        }

        [TestMethod]
        public void CompetitionIdValid()
        {
            // arrange
            Team team = new Team();
            team.CompetitionId = 1;

            // act
            int? result = team.CompetitionId;

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void CompetitionValid()
        {
            // arrange
            Team team = new Team();
            team.Competition = new Competition() { Id = 1, Name = "LaLiga" };

            Competition expectedSport = new Competition() { Id = 1, Name = "LaLiga" };

            // act
            var result = team.Competition;

            // assert
            Assert.AreEqual(expectedSport.Id, result.Id);
            Assert.AreEqual(expectedSport.Name, result.Name);
        }

        [TestMethod]
        public void StatisticValid()
        {
            // arrange
            Team team = new Team();
            team.Statistics = new List<Statistic>
            {
                new Statistic { Id = 1, TeamId = 1},
                new Statistic { Id = 2, TeamId = 2}
            };

            var expectedStatistics = new List<Statistic>
            {
                new Statistic { Id = 1, TeamId = 1},
                new Statistic { Id = 2, TeamId = 2}
            };

            // act
            var result = team.Statistics;

            // assert
            for (int i = 0; i < expectedStatistics.Count; i++)
            {
                Assert.AreEqual(expectedStatistics[i].Id, result[i].Id);
                Assert.AreEqual(expectedStatistics[i].TeamId, result[i].TeamId);
            }
        }

        [TestMethod]
        public void Results1Valid()
        {
            // arrange
            Team team = new Team();
            team.Results1 = new List<Result>
            {
                new Result { Id = 1, Team1Id = 1, Team2Id = 3},
                new Result { Id = 2, Team1Id = 2, Team2Id = 4}
            };

            var expectedResults = new List<Result>
            {
                new Result { Id = 1, Team1Id = 1, Team2Id = 3},
                new Result { Id = 2, Team1Id = 2, Team2Id = 4}
            };

            // act
            var result = team.Results1.ToList(); // Convert ICollection to List for index-based access

            // assert
            for (int i = 0; i < expectedResults.Count; i++)
            {
                Assert.AreEqual(expectedResults[i].Id, result[i].Id);
                Assert.AreEqual(expectedResults[i].Team1Id, result[i].Team1Id);
                Assert.AreEqual(expectedResults[i].Team2Id, result[i].Team2Id);
            }
        }

        [TestMethod]
        public void Results2Valid()
        {
            // arrange
            Team team = new Team();
            team.Results2 = new List<Result>
            {
                new Result { Id = 1, Team1Id = 1, Team2Id = 3},
                new Result { Id = 2, Team1Id = 2, Team2Id = 4}
            };

            var expectedResults = new List<Result>
            {
                new Result { Id = 1, Team1Id = 1, Team2Id = 3},
                new Result { Id = 2, Team1Id = 2, Team2Id = 4}
            };

            // act
            var result = team.Results2.ToList(); 

            // assert
            for (int i = 0; i < expectedResults.Count; i++)
            {
                Assert.AreEqual(expectedResults[i].Id, result[i].Id);
                Assert.AreEqual(expectedResults[i].Team1Id, result[i].Team1Id);
                Assert.AreEqual(expectedResults[i].Team2Id, result[i].Team2Id);
            }
        }
    }
}