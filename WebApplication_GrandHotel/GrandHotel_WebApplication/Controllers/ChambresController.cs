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

            ViewBag.stat = statusChambre;
            /*Requete sql qui recupère toutes les chambres ainsi que leur tarif reviser au 1er janvier de l'année en cours
             La requette est placé dans une vue dans la base GrandHotel*/

            string req = @"select Numero, Etage, NbLits,  Prix from vwChambresTarif";

            /*Requete sql qui recupère toutes les chambres occupées. La requette est placé dans une vue dans la base GrandHotel*/
            if (statusChambre == "Occupe") req = @"select Numero, Etage, NbLits,  Prix from vwChambresOccupeesTarif";

            /*Requete sql qui recupère toutes les chambres Livre. La requette est placé dans une vue dans la base GrandHotel*/
            else if (statusChambre == "NonOccupe")  req = @"select Numero, Etage, NbLits,  Prix from vwChambresLibreTarif";

            /*recupperation de la chaine de connexion dans une instruction using*/
            using (var conn = (SqlConnection)_context.Database.GetDbConnection())
            {
                /*Création de la commande et ajout de la code SQL et de la connexion*/
                var cmd = new SqlCommand(req, conn);

                /*Ouverture de la connexion*/
                await conn.OpenAsync();

                /*Lecture des lignes de résultat une par une*/
                using (var sdr = await cmd.ExecuteReaderAsync())
                {
                    while (sdr.Read())
                    {
                        /*L'enregistrement est placé dans un objet l'entité Chambre et l'objet est ajouté à la liste chambres*/
                        var chambre = new Chambre();
                        chambre.Numero = (short)sdr["Numero"];
                        chambre.Etage= (byte)sdr["Etage"];
                        chambre.NbLits= (byte)sdr["NbLits"];
                        chambre.Tarifc = (decimal)sdr["Prix"];
                        chambres.Add(chambre);
                    }
                }

            }
            /*L'instruction using appelle la méthode dispose de */
            vmChambre.Chambre = chambres;

            return View(vmChambre);

        }
        #endregion
        // GET: Chambres/Details/5
        //[Authorize]
        public IActionResult Details(short? id, string status)
        {
            ViewBag.DetailStatus = status;
            var vmChambre = new ChambreVM();
            var chambres = new Chambre();
            DateTime date = new DateTime(DateTime.Now.Year, 01, 01);
            if (id == null)
            {
                return NotFound();
            }
            var tarif = _context.TarifChambre
                .Include(tc => tc.NumChambreNavigation)
                .Include(tc=> tc.CodeTarifNavigation)
                .Where(tc => tc.CodeTarifNavigation.DateDebut >= date && tc.NumChambreNavigation.Numero == id).FirstOrDefault();
            if (chambres == null)
            {
                return NotFound();
            }
            ViewBag.id = id;
            vmChambre.TarifChambre = tarif;
            return View(vmChambre);
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
        public async Task<IActionResult> Edit(short? id, string status)
        {
            ViewBag.EditStatus = status;
            if (id == null)
            {
                return NotFound();
            }

            var chambre = await _context.Chambre.SingleOrDefaultAsync(m => m.Numero == id);
            if (chambre == null)
            {
                return NotFound();
            }
            ViewBag.id = id;
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
        public async Task<IActionResult> Delete(short? id, string status)
        {
         
            ViewBag.SupprimerStatus = status;
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
            ViewBag.id = id;
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
