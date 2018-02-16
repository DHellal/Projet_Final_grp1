using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GrandHotel_WebApplication.Data;
using GrandHotel_WebApplication.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
//using System.Data;

namespace GrandHotel_WebApplication.Controllers
{
    public class ChambresController : Controller
    {
        private readonly GrandHotelContext _context;

        public ChambresController(GrandHotelContext context)
        {
            _context = context;
        }
        #region Auteur : Lydia
        // GET: Chambres
        [Authorize]
        public async Task<IActionResult> Index(string statusChambre)
        {
            var vmChambre = new ChambreVM();
            var chambres = new List<Chambre>();
            //string req = "";
            if (string.IsNullOrWhiteSpace(statusChambre)) statusChambre = "";

            string req = @"select Numero, Prix from vwChambresTarif";

            if (statusChambre == "Occupe") req = @"select Numero, Prix from vwChambresOccupeesTarif";

            else if (statusChambre == "NonOccupe") req = @"select Numero, Prix from vwChambresLibreTarif";

            using (var conn = (SqlConnection)_context.Database.GetDbConnection())
            {
                var cmd = new SqlCommand(req, conn);
                await conn.OpenAsync();

                using (var sdr = await cmd.ExecuteReaderAsync())
                {
                    while (sdr.Read())
                    {
                        var chambre = new Chambre();
                        chambre.Numero = (short)sdr["Numero"];
                        chambre.Tarifc = (decimal)sdr["Prix"];
                        chambres.Add(chambre);
                    }
                }
            }

            vmChambre.Chambre = chambres;

            return View(vmChambre);

        }
        #endregion
        // GET: Chambres/Details/5
        [Authorize]
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chambre = await _context.Chambre
                .SingleOrDefaultAsync(m => m.Numero == id);
            if (chambre == null)
            {
                return NotFound();
            }

            return View(chambre);
        }

        // GET: Chambres/Create
        //[Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Chambres/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Numero,Etage,Bain,Douche,Wc,NbLits,NumTel")] Chambre chambre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chambre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chambre);
        }

        // GET: Chambres/Edit/5
        //[Authorize]
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chambre = await _context.Chambre.SingleOrDefaultAsync(m => m.Numero == id);
            if (chambre == null)
            {
                return NotFound();
            }
            return View(chambre);
        }

        // POST: Chambres/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("Numero,Etage,Bain,Douche,Wc,NbLits,NumTel")] Chambre chambre)
        {
            if (id != chambre.Numero)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chambre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChambreExists(chambre.Numero))
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
            return View(chambre);
        }

        // GET: Chambres/Delete/5
        //[Authorize]
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chambre = await _context.Chambre
                .SingleOrDefaultAsync(m => m.Numero == id);
            if (chambre == null)
            {
                return NotFound();
            }

            return View(chambre);
        }

        // POST: Chambres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var chambre = await _context.Chambre.SingleOrDefaultAsync(m => m.Numero == id);
            _context.Chambre.Remove(chambre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChambreExists(short id)
        {
            return _context.Chambre.Any(e => e.Numero == id);
        }
    }
}
