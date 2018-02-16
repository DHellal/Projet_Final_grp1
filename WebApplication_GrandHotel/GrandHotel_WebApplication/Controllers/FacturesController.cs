using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GrandHotel_WebApplication.Data;
using GrandHotel_WebApplication.Models;
using Microsoft.AspNetCore.Identity;
using GrandHotel_WebApplication.Outil;

namespace GrandHotel_WebApplication.Controllers
{
    public class FacturesController : Controller
    {
        private readonly GrandHotelContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FacturesController(GrandHotelContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return RedirectToAction("Afficher");
            }
        }

        // GET: Factures
        public async Task<IActionResult> Afficher(string AnneeEnCour, int page = 1)
        {

            var user = await _userManager.GetUserAsync(User);

            var email = user.Email;

            var clientId = await _context.Client.Where(e => e.Email == email).Select(i => i.Id).SingleOrDefaultAsync();


            if (AnneeEnCour == null)
            {
                AnneeEnCour = "2018";
            }

            int year = int.Parse(AnneeEnCour);

            var facturesVM = new FactureVM();

            //créer la liste de l'année : récupérer l'année dans BDD
            facturesVM.Factures = await _context.Facture.Where(f => f.IdClient == clientId).OrderByDescending(o => o.DateFacture).ToListAsync();
         
            List<int> years = new List<int>();
            //add l'année en cours
            int ThisYear = DateTime.Today.Year;
            years.Add(ThisYear);

            int yearToAdd = 0;

            foreach (var facture in facturesVM.Factures)
            {
                yearToAdd = facture.DateFacture.Year;
                if (!years.Contains(yearToAdd))
                {
                    years.Add(yearToAdd);
                }
            }
            ViewBag.Years = years;

            //liste factures selon l'année choisi
            facturesVM.Factures = await _context.Facture.Where(f => f.IdClient == clientId && f.DateFacture.Year == year).OrderByDescending(o => o.DateFacture).ToListAsync();
            facturesVM.AnneeEnCour = AnneeEnCour;

            return View(facturesVM);

        }
    }


    //// GET: Factures/Details/5
    //public async Task<IActionResult> Details(int? id)
    //{
    //    if (id == 0)
    //    {
    //        return NotFound();
    //    }

    //    // id = 1;

    //    var facture = await _context.Facture
    //        .Include(f => f.LigneFacture)
    //        .Where(l => l.Id == id)
    //        .SingleOrDefaultAsync();
    //    if (facture == null)
    //    {
    //        return NotFound();
    //    }

    //    return View(facture);
    //}

    //// GET: Factures/Create
    //public IActionResult Create()
    //{
    //    ViewData["CodeModePaiement"] = new SelectList(_context.ModePaiement, "Code", "Code");
    //    ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite");
    //    return View();
    //}

    //// POST: Factures/Create
    //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create([Bind("Id,IdClient,DateFacture,DatePaiement,CodeModePaiement")] Facture facture)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        _context.Add(facture);
    //        await _context.SaveChangesAsync();
    //        return RedirectToAction(nameof(Index));
    //    }
    //    ViewData["CodeModePaiement"] = new SelectList(_context.ModePaiement, "Code", "Code", facture.CodeModePaiement);
    //    ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite", facture.IdClient);
    //    return View(facture);
    //}

    //// GET: Factures/Edit/5
    //public async Task<IActionResult> Edit(int? id)
    //{
    //    if (id == null)
    //    {
    //        return NotFound();
    //    }

    //    var facture = await _context.Facture.SingleOrDefaultAsync(m => m.Id == id);
    //    if (facture == null)
    //    {
    //        return NotFound();
    //    }
    //    ViewData["CodeModePaiement"] = new SelectList(_context.ModePaiement, "Code", "Code", facture.CodeModePaiement);
    //    ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite", facture.IdClient);
    //    return View(facture);
    //}

    //// POST: Factures/Edit/5
    //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(int id, [Bind("Id,IdClient,DateFacture,DatePaiement,CodeModePaiement")] Facture facture)
    //{
    //    if (id != facture.Id)
    //    {
    //        return NotFound();
    //    }

    //    if (ModelState.IsValid)
    //    {
    //        try
    //        {
    //            _context.Update(facture);
    //            await _context.SaveChangesAsync();
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //            if (!FactureExists(facture.Id))
    //            {
    //                return NotFound();
    //            }
    //            else
    //            {
    //                throw;
    //            }
    //        }
    //        return RedirectToAction(nameof(Index));
    //    }
    //    ViewData["CodeModePaiement"] = new SelectList(_context.ModePaiement, "Code", "Code", facture.CodeModePaiement);
    //    ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite", facture.IdClient);
    //    return View(facture);
    //}

    //// GET: Factures/Delete/5
    //public async Task<IActionResult> Delete(int? id)
    //{
    //    if (id == null)
    //    {
    //        return NotFound();
    //    }

    //    var facture = await _context.Facture
    //        .Include(f => f.CodeModePaiementNavigation)
    //        .Include(f => f.IdClientNavigation)
    //        .SingleOrDefaultAsync(m => m.Id == id);
    //    if (facture == null)
    //    {
    //        return NotFound();
    //    }

    //    return View(facture);
    //}

    //// POST: Factures/Delete/5
    //[HttpPost, ActionName("Delete")]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> DeleteConfirmed(int id)
    //{
    //    var facture = await _context.Facture.SingleOrDefaultAsync(m => m.Id == id);
    //    _context.Facture.Remove(facture);
    //    await _context.SaveChangesAsync();
    //    return RedirectToAction(nameof(Index));
    //}

    //private bool FactureExists(int id)
    //{
    //    return _context.Facture.Any(e => e.Id == id);
    //}
}

