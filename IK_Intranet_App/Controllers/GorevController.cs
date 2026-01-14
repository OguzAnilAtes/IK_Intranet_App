using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IK_Intranet_App.Data;
using IK_Intranet_App.Models;
using Microsoft.AspNetCore.Authorization;

namespace IK_Intranet_App.Controllers
{
    [Authorize]
    public class GorevController : Controller
    {
        private readonly AppDbContext _context;

        public GorevController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Gorev
        public async Task<IActionResult> Index()
        {
            return View(await _context.Gorevler.ToListAsync());
        }

        // GET: Gorev/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gorev = await _context.Gorevler
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gorev == null)
            {
                return NotFound();
            }

            return View(gorev);
        }

        // GET: Gorev/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Gorev/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Baslik,Aciklama,AtananKisi,Durum")] Gorev gorev)
        {
            if (ModelState.IsValid)
            {
                gorev.OlusturmaTarihi = DateTime.UtcNow; // Tarihi biz basıyoruz
                _context.Add(gorev);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gorev);
        }

        // GET: Gorev/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gorev = await _context.Gorevler.FindAsync(id);
            if (gorev == null)
            {
                return NotFound();
            }
            return View(gorev);
        }

        // POST: Gorev/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Baslik,Aciklama,AtananKisi,Durum,OlusturmaTarihi")] Gorev gorev)
        {
            if (id != gorev.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gorev);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GorevExists(gorev.Id))
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
            return View(gorev);
        }

        // GET: Gorev/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gorev = await _context.Gorevler
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gorev == null)
            {
                return NotFound();
            }

            return View(gorev);
        }

        // POST: Gorev/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gorev = await _context.Gorevler.FindAsync(id);
            if (gorev != null)
            {
                _context.Gorevler.Remove(gorev);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GorevExists(int id)
        {
            return _context.Gorevler.Any(e => e.Id == id);
        }
    }
}
