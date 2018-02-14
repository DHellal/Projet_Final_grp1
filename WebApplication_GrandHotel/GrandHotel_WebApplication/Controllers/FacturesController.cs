using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GrandHotel_WebApplication.Data;
using GrandHotel_WebApplication.Models;

namespace GrandHotel_WebApplication.Controllers
{
    public class FacturesController : Controller
    {
        private readonly GrandHotelContext _context;

        public FacturesController(GrandHotelContext context)
        {
            _context = context;
        }

        // GET: Factures
        public async Task<IActionResult> Index(int id, string AnneeEnCour)
        {
            id = 1; // faut modifier selon idclient

            List<int> years = new List<int> { 2018, 2017, 2016, 2015 };
            ViewBag.Years = years;

            if (AnneeEnCour == null)
            {
                AnneeEnCour = "2018";
            }

            int year = int.Parse(AnneeEnCour);

            var facturesVM = new FactureVM();

            facturesVM.Factures= await _context.Facture.Where(f => f.IdClient == id && f.DateFacture.Year == year).OrderByDescending(o => o.DateFacture).ToListAsync();
            facturesVM.AnneeEnCour = AnneeEnCour;

            return View(facturesVM);
        }

        // GET: Factures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            // id = 1;

            var facture = await _context.Facture
                .Include(f => f.LigneFacture)
                .Where(l => l.Id == id)
                .SingleOrDefaultAsync();
            if (facture == null)
            {
                return NotFound();
            }

            return View(facture);
        }

        // GET: Factures/Create
        public IActionResult Create()
        {
            ViewData["CodeModePaiement"] = new SelectList(_context.ModePaiement, "Code", "Code");
            ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite");
            return View();
        }

        // POST: Factures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdClient,DateFacture,DatePaiement,CodeModePaiement")] Facture facture)
        {
            if (ModelState.IsValid)
            {
                _context.Add(facture);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CodeModePaiement"] = new SelectList(_context.ModePaiement, "Code", "Code", facture.CodeModePaiement);
            ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite", facture.IdClient);
            return View(facture);
        }

        // GET: Factures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facture = await _context.Facture.SingleOrDefaultAsync(m => m.Id == id);
            if (facture == null)
            {
                return NotFound();
            }
            ViewData["CodeModePaiement"] = new SelectList(_context.ModePaiement, "Code", "Code", facture.CodeModePaiement);
            ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite", facture.IdClient);
            return View(facture);
        }

        // POST: Factures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdClient,DateFacture,DatePaiement,CodeModePaiement")] Facture facture)
        {
            if (id != facture.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(facture);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FactureExists(facture.Id))
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
            ViewData["CodeModePaiement"] = new SelectList(_context.ModePaiement, "Code", "Code", facture.CodeModePaiement);
            ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite", facture.IdClient);
            return View(facture);
        }

        // GET: Factures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facture = await _context.Facture
                .Include(f => f.CodeModePaiementNavigation)
                .Include(f => f.IdClientNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (facture == null)
            {
                return NotFound();
            }

            return View(facture);
        }

        // POST: Factures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facture = await _context.Facture.SingleOrDefaultAsync(m => m.Id == id);
            _context.Facture.Remove(facture);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FactureExists(int id)
        {
            return _context.Facture.Any(e => e.Id == id);
        }
    }
}
