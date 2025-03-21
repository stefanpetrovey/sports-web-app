using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sports.Data;
using Sports.Models;

namespace Sports.Controllers
{
    public class CompetitionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompetitionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Competitions
        //<summary>
        // The id here represents the sportId which helps filter the 
        // competitions by their sport. The ViewBag.Id here is used to
        // help the create function filter the sport so that no other sport
        // can be added other than the sport for which the competition view
        // is for
        //</summary>
        public async Task<IActionResult> Index(int id)
        {
            ViewBag.Id = id;

            var applicationDbContext = _context.Competition.Include(c => c.Sport).Where(t => t.Sport.Id == id);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Competitions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competition = await _context.Competition
                .Include(c => c.Sport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (competition == null)
            {
                return NotFound();
            }

            return View(competition);
        }

        // GET: Competitions/Create
        //<summary>
        // The id here in the parameter is the ViewBag.Id from the index view.
        // Of course this is the sportId. The other ViewBag.Id here helps return
        // to the competitions index view with the correct competitions for the
        // sport selected. The availableSports variable filters to the current
        // sport we are in, so that no other sport can be added.
        //</summary>
        public IActionResult Create(int id)
        {
            ViewBag.Id = id;

            var availableSports = _context.Sport.Where(s => s.Id == id);

            ViewData["SportId"] = new SelectList(availableSports, "Id", "Name");

            return View();
        }

        // POST: Competitions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Country,SportId")] Competition competition, Sport sport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(competition);
                await _context.SaveChangesAsync();
                // new id set here to return to the correct view.
                return RedirectToAction(nameof(Index), new {id = competition.SportId});
            }

            var availableSports = _context.Sport.Include(s => s.Competitions).Where(s => s.Id == s.Competitions.FirstOrDefault().Id);

            ViewData["SportId"] = new SelectList(availableSports, "Id", "Name");

            return View(competition);
        }

        // GET: Competitions/Edit/5
        //<summary>
        // As before the first id here is the competition id and the second
        // is the sport id. I can't rename them competitionId and sportId
        // because the application does not work. id1 is mandatory for the edit
        // function and id2 is used to filter out the sports and return back to
        // the appropriate view. ViewBag.Id is used to return back.
        //</summary>
        public async Task<IActionResult> Edit(int? id1, int id2)
        {
            if (id1 == null)
            {
                return NotFound();
            }

            ViewBag.Id = id2;

            var competition = await _context.Competition.FindAsync(id1);
            if (competition == null)
            {
                return NotFound();
            }

            var availableSports = _context.Sport.Where(s => s.Id == id2);

            ViewData["SportId"] = new SelectList(availableSports, "Id", "Name", competition.SportId);
            return View(competition);
        }

        // POST: Competitions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Country,SportId")] Competition competition)
        {
            if (id != competition.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(competition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompetitionExists(competition.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // new id is set here to return to the correct view.
                return RedirectToAction(nameof(Index), new { id = competition.SportId });
            }

            var availableSports = _context.Sport.Where(s => s.Id == competition.SportId);

            ViewData["SportId"] = new SelectList(availableSports, "Id", "Name", competition.SportId);
            return View(competition);
        }

        // GET: Competitions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competition = await _context.Competition
                .Include(c => c.Sport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (competition == null)
            {
                return NotFound();
            }

            return View(competition);
        }

        // POST: Competitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var competition = await _context.Competition.FindAsync(id);
           
            if (competition != null)
            {
                _context.Competition.Remove(competition);
            }

            await _context.SaveChangesAsync();
            // new id is set here to return to the correct view.
            return RedirectToAction(nameof(Index), new { id = competition.SportId });
        }

        private bool CompetitionExists(int id)
        {
            return _context.Competition.Any(e => e.Id == id);
        }
    }
}