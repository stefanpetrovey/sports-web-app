using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sports.Data;
using Sports.Models;

namespace Sports.Controllers
{
    [Authorize(Roles = "Admin")]
    public class Statistic_TypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Statistic_TypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Statistic_Type
        public async Task<IActionResult> Index()
        {
            return View(await _context.Statistic_Type.ToListAsync());
        }

        // GET: Statistic_Type/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statistic_Type = await _context.Statistic_Type
                .FirstOrDefaultAsync(m => m.Id == id);
            if (statistic_Type == null)
            {
                return NotFound();
            }

            return View(statistic_Type);
        }

        // GET: Statistic_Type/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Statistic_Type/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Order_Of_Appearence")] Statistic_Type statistic_Type)
        {
            if (ModelState.IsValid)
            {
                _context.Add(statistic_Type);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(statistic_Type);
        }

        // GET: Statistic_Type/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statistic_Type = await _context.Statistic_Type.FindAsync(id);
            if (statistic_Type == null)
            {
                return NotFound();
            }
            return View(statistic_Type);
        }

        // POST: Statistic_Type/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Order_Of_Appearence")] Statistic_Type statistic_Type)
        {
            if (id != statistic_Type.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statistic_Type);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Statistic_TypeExists(statistic_Type.Id))
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
            return View(statistic_Type);
        }

        // GET: Statistic_Type/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statistic_Type = await _context.Statistic_Type
                .FirstOrDefaultAsync(m => m.Id == id);
            if (statistic_Type == null)
            {
                return NotFound();
            }

            return View(statistic_Type);
        }

        // POST: Statistic_Type/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var statistic_Type = await _context.Statistic_Type.FindAsync(id);
            if (statistic_Type != null)
            {
                _context.Statistic_Type.Remove(statistic_Type);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Statistic_TypeExists(int id)
        {
            return _context.Statistic_Type.Any(e => e.Id == id);
        }
    }
}