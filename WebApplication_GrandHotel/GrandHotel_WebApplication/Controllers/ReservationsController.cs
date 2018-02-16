﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GrandHotel_WebApplication.Data;
using GrandHotel_WebApplication.Models;
using System.Data.SqlClient;
using System.Data;
using GrandHotel_WebApplication.Extensions;
using Microsoft.AspNetCore.Identity;

namespace GrandHotel_WebApplication.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly GrandHotelContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationsController(GrandHotelContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Reservations
        public IActionResult Index()
        {

            return View();
        }

        // GET: Reservations/Details/5
        [Route("reservations/{id}/{prix}")]
        public async Task<IActionResult> Details(short id, decimal prix, DateTime jour, byte nbpersonne, bool? travail, int nbnuit, byte heure)
        {
            ViewBag.DetailJour = jour.Day;
            ViewBag.DetailMois = jour.Month;
            ViewBag.DetailAnnee = jour.Year;

            ViewBag.DetailNbPersonne = nbpersonne;
            ViewBag.DetailNbnuit = nbnuit;


            DateTime date = new DateTime(DateTime.Now.Year, 01, 01);
            ReservationVM chambreVM = new ReservationVM();
            chambreVM.tarifChambre = await _context
                .TarifChambre.Include(m => m.CodeTarifNavigation)
                .Include(m => m.NumChambreNavigation)
                .Where(m => m.NumChambre == id && m.CodeTarifNavigation.DateDebut >= date)
                .FirstOrDefaultAsync();
            chambreVM.tarifChambre.TarifTotal = prix;
            Guid g = Guid.NewGuid();
            var guid = g.ToString();
            ViewBag.guid = guid;
            var reservation = new Reservation();
            
            reservation.Jour = jour;
            reservation.NbPersonnes = nbpersonne;
            reservation.NumChambre = id;
            reservation.Travail = travail;
            reservation.HeureArrivee = heure;
            HttpContext.Session.SetObjectAsJson(guid, reservation);
           
            return View(chambreVM);
        }


        //GET: Reservations/Create
        
        public async Task<IActionResult> VerifDisponibilite(DateTime Jour, int NbNuit, byte NbPersonnes, byte HeureArrivee, bool? Travail)
        {

            ViewBag.Nbnuit = NbNuit;
            ViewBag.NbPersonnes = NbPersonnes;
            ViewBag.Jour = Jour.Day;
            ViewBag.Mois = Jour.Month;
            ViewBag.Annee = Jour.Year;
            ViewBag.HeureArrivee = HeureArrivee;
            ViewBag.Travail = Travail;

            var numeroChambre = _context.Chambre.Select(m => m.Numero).ToList();

            DateTime j = Jour;

            ReservationVM chambreVM = new ReservationVM();
            var numeroChambreOccupe = new List<int>();


            if (ModelState.IsValid)
            {
                using (var conn = (SqlConnection)_context.Database.GetDbConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    for (int i = 0; i < NbNuit; i++)
                    {
                        string req = @"select  r.NumChambre 
                                from reservation r
                                inner join chambre c on r.NumChambre=c.Numero
                                where c.NbLits>=@NbLits and r.Jour=@Jour";

                        var param = new SqlParameter { SqlDbType = SqlDbType.TinyInt, ParameterName = "@NbLits", Value = NbPersonnes };


                        var param1 = new SqlParameter { SqlDbType = SqlDbType.DateTime, ParameterName = "@Jour", Value = Jour };

                        var cmd = new SqlCommand(req, conn);

                        cmd.Parameters.Add(param);
                        cmd.Parameters.Add(param1);

                        using (var sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                var c = new Chambre();
                                if (sdr["NumChambre"] == DBNull.Value)
                                    break;
                                c.Numero = (short)sdr["NumChambre"];
                                numeroChambreOccupe.Add(c.Numero);
                            }
                        }
                        Jour = Jour.AddDays(1);
                    }
                    DateTime date = new DateTime(DateTime.Now.Year, 01, 01);
                    chambreVM.TarifChambre = await _context.TarifChambre
                        .Include(t => t.NumChambreNavigation)
                        .Include(t => t.CodeTarifNavigation)
                        .Where(x => !numeroChambreOccupe.Contains(x.NumChambre) && x.CodeTarifNavigation.DateDebut >= date)
                        .ToListAsync();
                }

            }


            return View(chambreVM);
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            string guid = ViewBag.guid;
            var user = await _userManager.GetUserAsync(User);
            var email=user.Email;
            var id = _context.Client.Where(c => c.Email == email).Select(c => c.Id).FirstOrDefault();
            var reservations = HttpContext.Session.GetObjectFromJson<Reservation>(guid);
            reservations.IdClient = id;
            _context.Add(reservations);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
           
            
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.SingleOrDefaultAsync(m => m.NumChambre == id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite", reservation.IdClient);
            ViewData["Jour"] = new SelectList(_context.Calendrier, "Jour", "Jour", reservation.Jour);
            ViewData["NumChambre"] = new SelectList(_context.Chambre, "Numero", "Numero", reservation.NumChambre);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("NumChambre,Jour,IdClient,NbPersonnes,HeureArrivee,Travail")] Reservation reservation)
        {
            if (id != reservation.NumChambre)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.NumChambre))
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
            ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite", reservation.IdClient);
            ViewData["Jour"] = new SelectList(_context.Calendrier, "Jour", "Jour", reservation.Jour);
            ViewData["NumChambre"] = new SelectList(_context.Chambre, "Numero", "Numero", reservation.NumChambre);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .Include(r => r.IdClientNavigation)
                .Include(r => r.JourNavigation)
                .Include(r => r.NumChambreNavigation)
                .SingleOrDefaultAsync(m => m.NumChambre == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var reservation = await _context.Reservation.SingleOrDefaultAsync(m => m.NumChambre == id);
            _context.Reservation.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(short id)
        {
            return _context.Reservation.Any(e => e.NumChambre == id);
        }

    
    }
}
   
