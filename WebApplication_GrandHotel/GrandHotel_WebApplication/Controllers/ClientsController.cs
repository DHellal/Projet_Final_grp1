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
using System.Data;
using GrandHotel_WebApplication.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using GrandHotel_WebApplication.Extensions;

namespace GrandHotel_WebApplication.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly GrandHotelContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private const string SessionKeyReservationVM = "_ReservationVM";

        public ClientsController(GrandHotelContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }
        #region Auteur: Lydia Réalisation: du 14 au 19 février 2018
        // GET: Clients
        public async Task<IActionResult> Index(char lettre = 'A')
        {
            var clients = new List<Client>();
            /*Requete sql qui recupère la liste des client et leurs nombres totaux de reservations, selon la première lettre
             de leurs noms*/
            string req = @"select C.Id, C.Civilite, C.Nom, C.Prenom, C.Email, count(R.IdClient) NbReservation
                           from Reservation R
                           right outer join Client C on C.Id = R.IdClient
                           where left(C.Nom,1)= @lettre 
                           group by C.Id, C.Civilite, C.Nom, C.Prenom, C.Email
                           order by C.Id";

            using (var conn = (SqlConnection)_context.Database.GetDbConnection())
            {
                var cmd = new SqlCommand(req, conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.Char,
                    ParameterName = "@lettre",
                    Value = lettre
                });
                await conn.OpenAsync();

                using (var sdr = await cmd.ExecuteReaderAsync())
                {
                    while (sdr.Read())
                    {
                        var client = new Client();
                        client.Id = (int)sdr["Id"];
                        client.Civilite = (string)sdr["Civilite"];
                        client.Nom = (string)sdr["Nom"];
                        client.Prenom = (string)sdr["Prenom"];
                        client.Email = (string)sdr["Email"];
                        client.NbReservation = (int)sdr["NbReservation"];

                        /*Requete sql qui recupère le nombre de reservation en cours de chaque client*/
                        req = @"select count(R.IdClient) NbReservationEnCours
                           from Reservation R
                           inner join Client C on C.Id = R.IdClient
                           where R.Jour>=GETDATE() And C.Id=@Id";
                        cmd = new SqlCommand(req, conn);
                        cmd.Parameters.Add(new SqlParameter
                        {
                            SqlDbType = SqlDbType.Int,
                            ParameterName = "@Id",
                            Value = client.Id
                        });
                        client.NbReservationEnCours = (int)cmd.ExecuteScalar();
                        clients.Add(client);
                        ViewBag.id = client.Id;
                    }
                }
            }
            ViewBag.stat = lettre;
            /*Requete Link to entity (EF) qui recupère la liste des client et leurs nombres totaux de reservations,
             selon la première lettre
           de leurs noms*/
            //var client = await _context.Client
            //    .Where(a => a.Nom.StartsWith(lettre))
            //    .Include(c => c.Reservation)
            //    .OrderBy(a => a.Id).ToListAsync();
            return View(clients);
        }
       
        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id, string status)
        {
            ViewBag.DetailStatus = status;
            if (id == null)
            {
                return NotFound();
            }

           
            var client = await _context.Client.SingleOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }
            ViewBag.id = id;
            return View(client);
        }
        #endregion
        #region Fonctionnalités à ajouter: en effet pour l'instant n'import quel utilisateur peut acceder au menu gestion. On ajoutera ces fonctionnalités si l'accès est limité aux gestionnaire par mesure de sécurité. 
        // GET: Clients/Create
        public IActionResult Create()
        {
            ModelState.Clear();
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreationClientVM clientVM)
        {

            if (ModelState.IsValid)
            {
                //Verification
                #region
                var user = await _userManager.GetUserAsync(User);
                string teldomi = await _context.Telephone.Where(t => t.Numero == clientVM.TelephoneDom).Select(t => t.Numero).SingleOrDefaultAsync();
                string telport = await _context.Telephone.Where(t => t.Numero == clientVM.TelephonePort).Select(t => t.Numero).SingleOrDefaultAsync();

                if (teldomi != null)
                {
                    clientVM.StatusMessage = "Numéro de telephone déja existant";
                    return View(clientVM);
                }

                if (telport != null)
                {
                    clientVM.StatusMessage = "Numéro de telephone déja existant";
                    return View(clientVM);
                }

                //Test si email unique

                Client clientAncien = _context.Client.Where(c => c.Email == user.Email).FirstOrDefault();
                if (clientAncien != null)
                {
                    clientVM.StatusMessage = "Erreur : Adresse Mail deja utilisée pour un compte...";
                    return View(clientVM);
                }
                #endregion

                //Création du client
                #region
                Client client = new Client
                {
                    Civilite = clientVM.Civilite,
                    Nom = clientVM.Nom.ToUpper(),
                    Prenom = clientVM.Prenom[0].ToString().ToUpper() + clientVM.Prenom.Substring(1),
                    Email = user.Email,
                    CarteFidelite = false
                };
                _context.Add(client);
                await _context.SaveChangesAsync();

                int id = _context.Client.OrderBy(c => c.Id).Select(c => c.Id).LastOrDefault();
                clientVM.id = id;
                #endregion

                //Création adresse
                #region
                if (clientVM.AdresseVille != null && clientVM.AdresseRue != null && clientVM.AdresseCodePostal != null)
                {

                    Adresse Adresse = new Adresse()
                    {
                        IdClient = clientVM.id,
                        Rue = clientVM.AdresseRue,
                        CodePostal = clientVM.AdresseCodePostal,
                        Ville = clientVM.AdresseVille.ToUpper()
                    };

                    _context.Add(Adresse);
                    await _context.SaveChangesAsync();
                }
                #endregion

                //Création/MAJ Telephone Domilcile
                #region
                if (clientVM.TelephoneDom.Length == 10)
                {
                    Telephone telDom = new Telephone()
                    {
                        IdClient = clientVM.id,
                        CodeType = "F",
                        Numero = clientVM.TelephoneDom,
                        Pro = clientVM.ProDom
                    };

                    if (teldomi == null)
                    {
                        _context.Add(telDom);
                        await _context.SaveChangesAsync();
                    }

                };
                #endregion

                //Création Telephone Portable
                #region
                if (clientVM.TelephonePort.Length == 10)
                {


                    Telephone telPort = new Telephone()
                    {
                        IdClient = clientVM.id,
                        CodeType = "M",
                        Numero = clientVM.TelephonePort,
                        Pro = clientVM.ProPort
                    };

                    if (telport == null)
                    {
                        _context.Add(telPort);
                        await _context.SaveChangesAsync();
                    }

                };
                #endregion

                var reservations = HttpContext.Session.GetObjectFromJson<Reservation>(SessionKeyReservationVM);

                //Si réussi, redirect vers change Account
                if (reservations != null)
                    return RedirectToAction("Creates", "Reservations");
                else
                {
                    clientVM.StatusMessage = "Bienvenue";
                    return RedirectToAction("ChangeAccount", "Manage", clientVM);
                }               
            }
            return View(clientVM);
        }


        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id, string status)
        {
            ViewBag.EditStatus = status;
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client.SingleOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
            //return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Civilite,Nom,Prenom,Email,CarteFidelite,Societe")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            return View(client);
        }

        

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id, string status)
        {
            ViewBag.SupprimerStatus = status;
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .SingleOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }
            ViewBag.id = id;
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Client.SingleOrDefaultAsync(m => m.Id == id);
            _context.Client.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
       
        private bool ClientExists(int id)
        {
            return _context.Client.Any(e => e.Id == id);
        }
        #endregion
    }
}
