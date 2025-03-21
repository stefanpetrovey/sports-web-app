using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Sports.Data;
using Sports.Interfaces;
using Sports.Models.StatisticCalculator;
using Sports.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.ConstrainedExecution;

namespace Sports.Controllers
{
    public class ResultsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public ResultsController(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        // GET: Results
        // <summary>
        // Like in the competitions controller this index here has a
        // competition id (id1), and a sport id (id2) in the parameter.
        // They are both assigned to the appropriate ViewBag and are used by
        // the create method. The create method uses them to display the right
        // teams and to successfully return back to the view that it originated
        // from. id1 is also used to filter out the team competition id, so that
        // teams from other competitions do not appear in the view. Also 
        // ViewBag.SportId is also used to return back to the correct 
        // competitions view.
        // </summary>
        public async Task<IActionResult> Index(int id1, int id2)
        {
            ViewBag.CompetitionId = id1;

            ViewBag.SportId = id2;

            var applicationDbContext = _context.Result.Include(r => r.Team1).Include(r => r.Team2).Include(r => r.Statistics).Where(r => r.Team1.Competition.Id == id1);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Results/Create
        //<summary>
        // Like mentioned above the two id's here are passed from the index
        // view and they are the competition id (id1) and the sport id (id2).
        // id1 helps the SelectList filter out the teams in the current
        // competition, and they are both assigned to a ViewBag once more
        // so that when returning back, the right index view is displayed.
        //</summary>
        public IActionResult Create(int id1, int id2)
        {
            var availableTeams = _context.Team.Include(t => t.Competition).Where(t => t.Competition.Id == id1);

            ViewBag.CompetitionId = id1;

            ViewBag.SportId = id2;

            ViewData["Team1Id"] = new SelectList(availableTeams, "Id", "Name");
            ViewData["Team2Id"] = new SelectList(availableTeams, "Id", "Name");
            return View();
        }

        // POST: Results/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // <summary>
        // The create post method first of all includes a backend validation
        // to check if the teams contain the same id. If not it will let the
        // user add a match. Then we have a few variables that we later use
        // to calculate the statistics. Also this is one of the methods that
        // uses our factory pattern. Id parameters are not used here because
        // it throws a double id exception. Caching is also used here to
        // clear and repopulate.
        // </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Team1Id,Team2Id,Team1Result,Team2Result")] Result result, Statistic statistic)
        {
            if (result.Team1Id == result.Team2Id)
            {
                ModelState.AddModelError("Team1Id", "A team cannot play against itself. Please select a different one.");
                ModelState.AddModelError("Team2Id", "A team cannot play against itself. Please select a different one.");
            }

            var team1Matches = await _context.Result
                .Where(r => r.Team1Id == result.Team1Id)
                .ToListAsync();

            var team2Matches = await _context.Result
               .Where(r => r.Team2Id == result.Team2Id)
               .ToListAsync();

            var statisticsTeam1 = await _context.Statistic
                .Where(x => x.TeamId == result.Team1Id)
                .Include(x => x.StatisticType)
                .ToListAsync();

            var statisticsTeam2 = await _context.Statistic
                .Where(x => x.TeamId == result.Team2Id)
                .Include(x => x.StatisticType)
                .ToListAsync();

            if (statisticsTeam1 == null || statisticsTeam2 == null) 
                return Content("error");

            double teamMatchesTeam1 = await _context.Result
                    .Where(r => r.Team1Id == result.Team1Id || r.Team2Id == result.Team1Id)
                    .CountAsync();

            double teamMatchesWithWinsTeam1 = await _context.Result
                .Where(r => (r.Team1Id == result.Team1Id && r.Team1Result > r.Team2Result)
                         || (r.Team2Id == result.Team1Id && r.Team2Result > r.Team1Result))
                .CountAsync();

            double teamMatchesTeam2 = await _context.Result
                        .Where(r => r.Team1Id == result.Team2Id || r.Team2Id == result.Team2Id)
                        .CountAsync();

            double teamMatchesWithWinsTeam2 = await _context.Result
                .Where(r => (r.Team1Id == result.Team2Id && r.Team1Result > r.Team2Result)
                         || (r.Team2Id == result.Team2Id && r.Team2Result > r.Team1Result))
                .CountAsync();

            result.Team1 = await _context.Team
                    .Include(t => t.Competition)
                    .ThenInclude(t => t.Sport)
                    .FirstOrDefaultAsync(t => t.Id == result.Team1Id);

            if(result.Team1.Competition.Sport != null)
            {
                var sport = result.Team1.Competition.Sport.Name;

                StatisticCalculatorFactory statisticCalculatorFactory = new StatisticCalculatorFactory();

                IStatisticCalculator soccer = statisticCalculatorFactory.GetStatistic("Soccer");

                soccer.CalculateStatisticsTeam1(team1Matches, statisticsTeam1, result, teamMatchesTeam1, teamMatchesWithWinsTeam1, sport);

                soccer.CalculateStatisticsTeam2(team2Matches, statisticsTeam2, result, teamMatchesTeam2, teamMatchesWithWinsTeam2, sport);

                IStatisticCalculator basketball = statisticCalculatorFactory.GetStatistic("Basketball");

                basketball.CalculateStatisticsTeam1(team1Matches, statisticsTeam1, result, teamMatchesTeam1, teamMatchesWithWinsTeam1, sport);

                basketball.CalculateStatisticsTeam2(team2Matches, statisticsTeam2, result, teamMatchesTeam2, teamMatchesWithWinsTeam2, sport);

                IStatisticCalculator handball = statisticCalculatorFactory.GetStatistic("Handball");

                handball.CalculateStatisticsTeam1(team1Matches, statisticsTeam1, result, teamMatchesTeam1, teamMatchesWithWinsTeam1, sport);

                handball.CalculateStatisticsTeam2(team2Matches, statisticsTeam2, result, teamMatchesTeam2, teamMatchesWithWinsTeam2, sport);

                IStatisticCalculator volleyball = statisticCalculatorFactory.GetStatistic("Volleyball");

                volleyball.CalculateStatisticsTeam1(team1Matches, statisticsTeam1, result, teamMatchesTeam1, teamMatchesWithWinsTeam1, sport);

                volleyball.CalculateStatisticsTeam2(team2Matches, statisticsTeam2, result, teamMatchesTeam2, teamMatchesWithWinsTeam2, sport);

                IStatisticCalculator hockey = statisticCalculatorFactory.GetStatistic("Hockey");

                hockey.CalculateStatisticsTeam1(team1Matches, statisticsTeam1, result, teamMatchesTeam1, teamMatchesWithWinsTeam1, sport);

                hockey.CalculateStatisticsTeam2(team2Matches, statisticsTeam2, result, teamMatchesTeam2, teamMatchesWithWinsTeam2, sport);
            }

            if (ModelState.IsValid)
            {
                _context.Add(result);
                await _context.SaveChangesAsync();

                var CompName = result.Team1.Competition.Name.Replace(" ", "");
                var SportName = result.Team1.Competition.Sport.Name.Replace(" ", "");
                var cacheName = $"{SportName}:{CompName}";

                // remove cache and repopulate
                _memoryCache.Remove(cacheName);
                var updatedStatistics = await _context.Statistic
                    .Include(s => s.Competition)
                    .Include(s => s.StatisticType)
                    .Include(s => s.Team)
                    .Where(s => s.Competition.Id == result.Team1.Competition.Id)
                    .ToListAsync();

                MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions();
                cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(1));
                cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                cacheOptions.SetPriority(CacheItemPriority.High);

                _memoryCache.Set(cacheName, updatedStatistics, cacheOptions);

                result.Team1 = await _context.Team
                    .Include(t => t.Competition)
                    .FirstOrDefaultAsync(t => t.Id == result.Team1Id);

                if (result.Team1.Competition != null)
                {
                    // new id's to return to the appropriate view
                    return RedirectToAction(nameof(Index), new { id1 = result.Team1.Competition.Id, id2 = result.Team1.Competition.SportId });
                }
                return Content("Error: Competition information not found.");
            }

            if (result.Team1Id != 0)
            {
                result.Team1 = await _context.Team
                    .Include(t => t.Competition)
                    .FirstOrDefaultAsync(t => t.Id == result.Team1Id);

                if (result.Team1 != null && result.Team1.Competition != null)
                {
                    // updated ViewBags in case of a validation occurence
                    ViewBag.CompetitionId = result.Team1.Competition.Id;
                    ViewBag.SportId = result.Team1.Competition.SportId;

                    var availableTeams = _context.Team
                        .Include(t => t.Competition)
                        .Where(t => t.Competition.Id == result.Team1.Competition.Id);

                    ViewData["Team1Id"] = new SelectList(availableTeams, "Id", "Name", result.Team1Id);
                    ViewData["Team2Id"] = new SelectList(availableTeams, "Id", "Name", result.Team2Id);
                }
                else
                {
                    return Content("Error: Team1 or its Competition not found in case of validation failure.");
                }
            }
            return View(result);
        }

        // <summary>
        // Just like in the other methods, here id1 is the competition id,
        // id2 is the sport id, id1 helps filter out the teams that are in the
        // competition, id2 is assigned to ViewBag.Id and is used to return to
        // the competitions index view. 
        // </summary
        [HttpGet]
        public IActionResult CreateViewForCompetition(int id1, int id2)
        {
            var availableTeams = _context.Team.Include(t => t.Competition).Where(t => t.Competition.Id == id1).ToList();

            ViewBag.Id = id2;

            ViewData["Team1Id"] = new SelectList(availableTeams, "Id", "Name");
            ViewData["Team2Id"] = new SelectList(availableTeams, "Id", "Name");
            return View();
        }

        // POST: Results/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // <summary>
        // This method is just a copy of the first one but is accesed directly
        // from the competitions index view instead of the results index view.
        // Caching is also used here to clear and repopulate.
        // No major changes from the first create method.
        // </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateViewForCompetition([Bind("Team1Id,Team2Id,Team1Result,Team2Result")] Result result, Statistic statistic)
        {
            if (result.Team1Id == result.Team2Id)
            {
                ModelState.AddModelError("Team1Id", "A team cannot play against itself. Please select a different one.");
                ModelState.AddModelError("Team2Id", "A team cannot play against itself. Please select a different one.");
            }

            var team1Matches = await _context.Result
                .Where(r => r.Team1Id == result.Team1Id)
                .ToListAsync();

            var team2Matches = await _context.Result
               .Where(r => r.Team2Id == result.Team2Id)
               .ToListAsync();

            var statisticsTeam1 = await _context.Statistic
                 .Where(x => x.TeamId == result.Team1Id)
                 .Include(x => x.StatisticType)
                 .ToListAsync();

            var statisticsTeam2 = await _context.Statistic
                .Where(x => x.TeamId == result.Team2Id)
                .Include(x => x.StatisticType)
                .ToListAsync();

            if (statisticsTeam1 == null || statisticsTeam2 == null)
                return Content("error");

            double teamMatchesTeam1 = await _context.Result
                        .Where(r => r.Team1Id == result.Team1Id || r.Team2Id == result.Team1Id)
                    .CountAsync();

            double teamMatchesWithWinsTeam1 = await _context.Result
                .Where(r => (r.Team1Id == result.Team1Id && r.Team1Result > r.Team2Result)
                         || (r.Team2Id == result.Team1Id && r.Team2Result > r.Team1Result))
                .CountAsync();

            double teamMatchesTeam2 = await _context.Result
                        .Where(r => r.Team1Id == result.Team2Id || r.Team2Id == result.Team2Id)
                        .CountAsync();

            double teamMatchesWithWinsTeam2 = await _context.Result
                .Where(r => (r.Team1Id == result.Team2Id && r.Team1Result > r.Team2Result)
                         || (r.Team2Id == result.Team2Id && r.Team2Result > r.Team1Result))
                .CountAsync();

            result.Team1 = await _context.Team
                    .Include(t => t.Competition)
                    .ThenInclude(t => t.Sport)
                    .FirstOrDefaultAsync(t => t.Id == result.Team1Id);

            if (result.Team1.Competition.Sport != null)
            {
                var sport = result.Team1.Competition.Sport.Name;

                StatisticCalculatorFactory statisticCalculatorFactory = new StatisticCalculatorFactory();

                IStatisticCalculator soccer = statisticCalculatorFactory.GetStatistic("Soccer");

                soccer.CalculateStatisticsTeam1(team1Matches, statisticsTeam1, result, teamMatchesTeam1, teamMatchesWithWinsTeam1, sport);

                soccer.CalculateStatisticsTeam2(team2Matches, statisticsTeam2, result, teamMatchesTeam2, teamMatchesWithWinsTeam2, sport);

                IStatisticCalculator basketball = statisticCalculatorFactory.GetStatistic("Basketball");

                basketball.CalculateStatisticsTeam1(team1Matches, statisticsTeam1, result, teamMatchesTeam1, teamMatchesWithWinsTeam1, sport);

                basketball.CalculateStatisticsTeam2(team2Matches, statisticsTeam2, result, teamMatchesTeam2, teamMatchesWithWinsTeam2, sport);

                IStatisticCalculator handball = statisticCalculatorFactory.GetStatistic("Handball");

                handball.CalculateStatisticsTeam1(team1Matches, statisticsTeam1, result, teamMatchesTeam1, teamMatchesWithWinsTeam1, sport);

                handball.CalculateStatisticsTeam2(team2Matches, statisticsTeam2, result, teamMatchesTeam2, teamMatchesWithWinsTeam2, sport);

                IStatisticCalculator volleyball = statisticCalculatorFactory.GetStatistic("Volleyball");

                volleyball.CalculateStatisticsTeam1(team1Matches, statisticsTeam1, result, teamMatchesTeam1, teamMatchesWithWinsTeam1, sport);

                volleyball.CalculateStatisticsTeam2(team2Matches, statisticsTeam2, result, teamMatchesTeam2, teamMatchesWithWinsTeam2, sport);

                IStatisticCalculator hockey = statisticCalculatorFactory.GetStatistic("Hockey");

                hockey.CalculateStatisticsTeam1(team1Matches, statisticsTeam1, result, teamMatchesTeam1, teamMatchesWithWinsTeam1, sport);

                hockey.CalculateStatisticsTeam2(team2Matches, statisticsTeam2, result, teamMatchesTeam2, teamMatchesWithWinsTeam2, sport);
            }

            if (ModelState.IsValid)
            {
                _context.Add(result);
                await _context.SaveChangesAsync();

                var CompName = result.Team1.Competition.Name.Replace(" ", "");
                var SportName = result.Team1.Competition.Sport.Name.Replace(" ", "");
                var cacheName = $"{SportName}:{CompName}";

                // remove cache and repopulate
                _memoryCache.Remove(cacheName);
                var updatedStatistics = await _context.Statistic
                    .Include(s => s.Competition)
                    .Include(s => s.StatisticType)
                    .Include(s => s.Team)
                    .Where(s => s.Competition.Id == result.Team1.Competition.Id)
                    .ToListAsync();

                MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions();
                cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(1));
                cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                cacheOptions.SetPriority(CacheItemPriority.High);

                _memoryCache.Set(cacheName, updatedStatistics, cacheOptions);

                if (result.Team1.Competition != null)
                {
                    return RedirectToAction(nameof(Index), new { id1 = result.Team1.Competition.Id, id2 = result.Team1.Competition.SportId });
                }
                return Content("Error: Competition information not found.");
            }

            if (result.Team1Id != 0)
            {
                result.Team1 = await _context.Team
                    .Include(t => t.Competition)
                    .FirstOrDefaultAsync(t => t.Id == result.Team1Id);

                if (result.Team1 != null && result.Team1.Competition != null)
                {
                    ViewBag.Id = result.Team1.Competition.SportId;

                    var availableTeams = _context.Team
                        .Include(t => t.Competition)
                        .Where(t => t.Competition.Id == result.Team1.Competition.Id);

                    ViewData["Team1Id"] = new SelectList(availableTeams, "Id", "Name", result.Team1Id);
                    ViewData["Team2Id"] = new SelectList(availableTeams, "Id", "Name", result.Team2Id);
                }
                else
                {
                    return Content("Error: Team1 or its Competition not found in case of validation failure.");
                }
            }
            return View(result);
        }

        // GET: Results/Delete/5
        // <summary>
        // Again, this is just the same as the other methods except here the
        // first id (id1) is the default delete id, which is the result id.
        // id2 is the competition id and id3 is the sport id. ViewBags are also
        // assigned so that we can return to the correct result index view.
        // </summary>
        public async Task<IActionResult> Delete(int? id1, int id2, int id3)
        {
            if (id1 == null)
            {
                return NotFound();
            }

            var result = await _context.Result
                .Include(r => r.Team1)
                .Include(r => r.Team2)
                .FirstOrDefaultAsync(m => m.Id == id1);

            ViewBag.CompetitionId = id2;

            ViewBag.SportId = id3;

            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        // POST: Results/Delete/5
        // <summary
        // This method's variables are exactly the same as the two create ones
        // except our factory methods here, instead of calculating the
        // statistics, they are deleting them. That's the only major difference
        // here. Caching is also used here to clear and repopulate.
        // </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, Result? result2, Statistic statistic)
        {
            result2 = await _context.Result.FindAsync(id);
    
            if (result2 != null)
            {
                var team1Matches = await _context.Result
                .Where(r => r.Team1Id == result2.Team1Id)
                .ToListAsync();

                var team2Matches = await _context.Result
                   .Where(r => r.Team2Id == result2.Team2Id)
                   .ToListAsync();

                var statisticsTeam1 = await _context.Statistic
                 .Where(x => x.TeamId == result2.Team1Id)
                 .Include(x => x.StatisticType)
                 .ToListAsync();

                var statisticsTeam2 = await _context.Statistic
                    .Where(x => x.TeamId == result2.Team2Id)
                    .Include(x => x.StatisticType)
                    .ToListAsync();

                if (statisticsTeam1 == null && statisticsTeam2 == null) 
                    return Content("error");

                double teamMatchesTeam1 = await _context.Result
                        .Where(r => r.Team1Id == result2.Team1Id || r.Team2Id == result2.Team1Id)
                    .CountAsync();

                double teamMatchesWithWinsTeam1 = await _context.Result
                    .Where(r => (r.Team1Id == result2.Team1Id && r.Team1Result > r.Team2Result)
                             || (r.Team2Id == result2.Team1Id && r.Team2Result > r.Team1Result))
                    .CountAsync();

                double teamMatchesTeam2 = await _context.Result
                            .Where(r => r.Team1Id == result2.Team2Id || r.Team2Id == result2.Team2Id)
                            .CountAsync();

                double teamMatchesWithWinsTeam2 = await _context.Result
                    .Where(r => (r.Team1Id == result2.Team2Id && r.Team1Result > r.Team2Result)
                             || (r.Team2Id == result2.Team2Id && r.Team2Result > r.Team1Result))
                    .CountAsync();

                result2.Team1 = await _context.Team
                    .Include(t => t.Competition)
                    .ThenInclude(t => t.Sport)
                    .FirstOrDefaultAsync(t => t.Id == result2.Team1Id);

                if (result2.Team1.Competition.Sport != null)
                {
                    var sport = result2.Team1.Competition.Sport.Name;

                    StatisticCalculatorFactory statisticCalculatorFactory = new StatisticCalculatorFactory();

                    IStatisticCalculator soccer = statisticCalculatorFactory.GetStatistic("Soccer");

                    soccer.DeleteStatisticsTeam1(team1Matches, statisticsTeam1, result2, teamMatchesTeam1, teamMatchesWithWinsTeam1, sport);

                    soccer.DeleteStatisticsTeam2(team2Matches, statisticsTeam2, result2, teamMatchesTeam2, teamMatchesWithWinsTeam2, sport);

                    IStatisticCalculator basketball = statisticCalculatorFactory.GetStatistic("Basketball");

                    basketball.DeleteStatisticsTeam1(team1Matches, statisticsTeam1, result2, teamMatchesTeam1, teamMatchesWithWinsTeam1, sport);

                    basketball.DeleteStatisticsTeam2(team2Matches, statisticsTeam2, result2, teamMatchesTeam2, teamMatchesWithWinsTeam2, sport);

                    IStatisticCalculator handball = statisticCalculatorFactory.GetStatistic("Handball");

                    handball.DeleteStatisticsTeam1(team1Matches, statisticsTeam1, result2, teamMatchesTeam1, teamMatchesWithWinsTeam1, sport);

                    handball.DeleteStatisticsTeam2(team2Matches, statisticsTeam2, result2, teamMatchesTeam2, teamMatchesWithWinsTeam2, sport);

                    IStatisticCalculator volleyball = statisticCalculatorFactory.GetStatistic("Volleyball");

                    volleyball.DeleteStatisticsTeam1(team1Matches, statisticsTeam1, result2, teamMatchesTeam1, teamMatchesWithWinsTeam1, sport);

                    volleyball.DeleteStatisticsTeam2(team2Matches, statisticsTeam2, result2, teamMatchesTeam2, teamMatchesWithWinsTeam2, sport);

                    IStatisticCalculator hockey = statisticCalculatorFactory.GetStatistic("Hockey");

                    hockey.DeleteStatisticsTeam1(team1Matches, statisticsTeam1, result2, teamMatchesTeam1, teamMatchesWithWinsTeam1, sport);

                    hockey.DeleteStatisticsTeam2(team2Matches, statisticsTeam2, result2, teamMatchesTeam2, teamMatchesWithWinsTeam2, sport);
                }

                _context.Result.Remove(result2);

                var CompName = result2.Team1.Competition.Name.Replace(" ", "");
                var SportName = result2.Team1.Competition.Sport.Name.Replace(" ", "");
                var cacheName = $"{SportName}:{CompName}";

                // remove cache and repopulate
                _memoryCache.Remove(cacheName);
                var updatedStatistics = await _context.Statistic
                    .Include(s => s.Competition)
                    .Include(s => s.StatisticType)
                    .Include(s => s.Team)
                    .Where(s => s.Competition.Id == result2.Team1.Competition.Id)
                    .ToListAsync();

                MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions();
                cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(1));
                cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                cacheOptions.SetPriority(CacheItemPriority.High);

                _memoryCache.Set(cacheName, updatedStatistics, cacheOptions);
            }

            await _context.SaveChangesAsync();
            // new id's here to return to the correct view
            return RedirectToAction(nameof(Index), new { id1 = result2.Team1.Competition.Id, id2 = result2.Team1.Competition.SportId });
        }

        private bool ResultExists(int id)
        {
            return _context.Result.Any(e => e.Id == id);
        }
    }
}