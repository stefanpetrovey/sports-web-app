using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Sports.Data;
using Sports.Models;

namespace Sports.Controllers
{
    
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public StatisticsController(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        // GET: Statistics
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Statistic.Include(s => s.Competition).Include(s => s.StatisticType).Include(s => s.Team);
            return View(await applicationDbContext.ToListAsync());
        }

        // <summary>
        // This view is for the regular user. id1 filters the competition
        // in which the teams are playing and id2 is the sportId which helps
        // return to the competitions view for the correct sport.
        // Caching is also implemented. The method checks if the data (statistics)
        // for the given competition is already cached using
        // _memoryCache.TryGetValue(cacheName, out statistics).
        // If it is, the cached data is returned. If no cache is found,
        // the method fetches the statistics from the database.
        // </summary>
        public async Task<IActionResult> IndexForCompetitions(int id1, int id2)
        {
            ViewBag.Id = id2;

            List<Statistic> statistics = new();
            var dbcontext = _context.Statistic
                .Include(s => s.Competition)
                .Include(s => s.Competition.Sport)
                .Where(s => s.Competition.Id == id1);

            var list = await dbcontext.ToListAsync();
            var compName = list[0].Competition.Name.Replace(" ", "");
            var sportName = list[0].Competition.Sport.Name.Replace(" ", "");
            var cacheName = $"{sportName}:{compName}";

            // try to get data from cache
            if (!_memoryCache.TryGetValue(cacheName, out statistics))
            {
                statistics = await _context.Statistic
                    .Include(s => s.Competition)
                    .Include(s => s.StatisticType)
                    .Include(s => s.Team)
                    .Where(s => s.Competition.Id == id1)
                    .ToListAsync();

                MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions();
                cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(1));
                cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                cacheOptions.SetPriority(CacheItemPriority.High);

                _memoryCache.Set(cacheName, statistics, cacheOptions);
            }
            // return cached data
            return View(statistics);
        }

        // GET: Statistics/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statistic = await _context.Statistic
                .Include(s => s.Competition)
                .Include(s => s.StatisticType)
                .Include(s => s.Team)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (statistic == null)
            {
                return NotFound();
            }

            return View(statistic);
        }

        // GET: Statistics/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CompetitionId"] = new SelectList(_context.Competition, "Id", "Name");
            ViewData["Statistic_TypeId"] = new SelectList(_context.Set<Statistic_Type>(), "Id", "Name");
            ViewData["TeamId"] = new SelectList(_context.Team, "Id", "Name");
            return View();
        }

        // POST: Statistics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // <summary>
        // This create method adds a statistic but it adds the statistic to
        // every team that is part of the competition that was selected. Caching
        // here is also present. When a new statistic is successfully added
        // to the database, the cache must be updated.
        // </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,TeamId,CompetitionId,Value,Statistic_TypeId")] Statistic statistic, Result result)
        {
            var competition = await _context.Competition
                .Include(s => s.Teams)
                .Include(s => s.Sport)
                .FirstOrDefaultAsync(x => x.Id == statistic.CompetitionId);
            
            if (competition.Teams.Count == 0)
                return Content("No teams in this competition, cannot add statistic");

            if (ModelState.IsValid)
            {
                // add all the teams in a competition
                foreach (var team in competition.Teams)
                {
                    var statisticToAdd = new Statistic
                    {
                        TeamId = team.Id,
                        CompetitionId = statistic.CompetitionId,
                        Statistic_TypeId = statistic.Statistic_TypeId,
                    };

                    _context.Add(statisticToAdd);
                }

                await _context.SaveChangesAsync();

                var compName = competition.Name.Replace(" ", "");
                var sportName = competition.Sport.Name.Replace(" ", "");
                var cacheName = $"{sportName}:{compName}";

                _memoryCache.Remove(cacheName);

                var updatedStatistics = await _context.Statistic
                    .Include(s => s.Competition)
                    .Include(s => s.StatisticType)
                    .Include(s => s.Team)
                    .Where(s => s.Competition.Id == statistic.CompetitionId)
                    .ToListAsync();

                MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(2))
                    .SetPriority(CacheItemPriority.High);

                _memoryCache.Set(cacheName, updatedStatistics, cacheOptions);

                return RedirectToAction(nameof(Index));
            }

            ViewData["CompetitionId"] = new SelectList(_context.Competition, "Id", "Name", statistic.CompetitionId);
            ViewData["Statistic_TypeId"] = new SelectList(_context.Statistic_Type, "Id", "Name", statistic.Statistic_TypeId);
            ViewData["TeamId"] = new SelectList(_context.Team, "Id", "Name", statistic.TeamId);
            return View(statistic);
        }

        // GET: Statistics/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statistic = await _context.Statistic.FindAsync(id);
            if (statistic == null)
            {
                return NotFound();
            }
            ViewData["CompetitionId"] = new SelectList(_context.Competition, "Id", "Name", statistic.CompetitionId);
            ViewData["Statistic_TypeId"] = new SelectList(_context.Set<Statistic_Type>(), "Id", "Name", statistic.Statistic_TypeId);
            ViewData["TeamId"] = new SelectList(_context.Team, "Id", "Name", statistic.TeamId);
            return View(statistic);
        }

        // POST: Statistics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TeamId,CompetitionId,Value,Statistic_TypeId")] Statistic statistic)
        {
            if (id != statistic.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statistic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatisticExists(statistic.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompetitionId"] = new SelectList(_context.Competition, "Id", "Name", statistic.CompetitionId);
            ViewData["Statistic_TypeId"] = new SelectList(_context.Set<Statistic_Type>(), "Id", "Name", statistic.Statistic_TypeId);
            ViewData["TeamId"] = new SelectList(_context.Team, "Id", "Name", statistic.TeamId);
            return View(statistic);
        }

        // GET: Statistics/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statistic = await _context.Statistic
                .Include(s => s.Competition)
                .Include(s => s.StatisticType)
                .Include(s => s.Team)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (statistic == null)
            {
                return NotFound();
            }

            return View(statistic);
        }

        // POST: Statistics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        // <summary>
        // The only thing here worth noting is the caching.
        // Similar to the Create method, the cache is invalidated
        // for that specific competition by removing
        // the existing cache entry (_memoryCache.Remove(cacheName)).
        // </summary>
        public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var statistic = await _context.Statistic.FindAsync(id);
                if (statistic != null)
                {
                    _context.Statistic.Remove(statistic);
                }

                await _context.SaveChangesAsync();

                var competition = await _context.Competition
                    .Include(s => s.Teams)
                    .Include(s => s.Sport)
                    .FirstOrDefaultAsync(x => x.Id == statistic.CompetitionId);

                if (competition.Teams.Count == 0)
                    return Content("No teams in this competition");

                var compName = competition.Name.Replace(" ", "");
                var sportName = competition.Sport.Name.Replace(" ", "");
                var cacheName = $"{sportName}:{compName}";

                _memoryCache.Remove(cacheName);

                var updatedStatistics = await _context.Statistic
                    .Include(s => s.Competition)
                    .Include(s => s.StatisticType)
                    .Include(s => s.Team)
                    .Where(s => s.Competition.Id == statistic.CompetitionId)
                    .ToListAsync();

                MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(2))
                    .SetPriority(CacheItemPriority.High);

                _memoryCache.Set(cacheName, updatedStatistics, cacheOptions);

                return RedirectToAction(nameof(Index));
            }

        [Authorize(Roles = "Admin")]
        private bool StatisticExists(int id)
        {
            return _context.Statistic.Any(e => e.Id == id);
        }
    }
}