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
using GrandHotel_WebApplication.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace GrandHotel_WebApplication.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly GrandHotelContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientsController(GrandHotelContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index(char lettre = 'A')
        {
            var clients = new List<Client>();

            string req = @"select C.Id, C.Civilite, C.Nom, C.Prenom, C.Email, count(R.IdClient) NbReservation
                           from Reservation R
                           left outer join Client C on C.Id = R.IdClient
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
                        var cli = new Client();
                        cli.Id = (int)sdr["Id"];
                        cli.Civilite = (string)sdr["Civilite"];
                        cli.Nom = (string)sdr["Nom"];
                        cli.Prenom = (string)sdr["Prenom"];
                        cli.Email = (string)sdr["Email"];
                        cli.NbReservation = (int)sdr["NbReservation"];
                        clients.Add(cli);
                    }
                }
            }

            //var client = await _context.Client
            //    .Where(a => a.Nom.StartsWith(lettre))
            //    .Include(c => c.Reservation)
            //    .OrderBy(a => a.Id).ToListAsync();
            return View(clients);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var client = await _context.Client
            //    .SingleOrDefaultAsync(m => m.Id == id);

            var client = new Client();
            string req = @"select C.Id,C.Civilite, C.Nom, C.Prenom, C.Email,count(R.IdClient) NbReservationEnCours
                           from Reservation R
                           left outer join Client C on C.Id = R.IdClient
                           where R.Jour>=GETDATE() And C.Id=@Id
                           group by C.Id, C.Civilite, C.Nom, C.Prenom, C.Email";

            using (var conn = (SqlConnection)_context.Database.GetDbConnection())
            {
                var cmd = new SqlCommand(req, conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@Id",
                    Value = id
                });
                await conn.OpenAsync();

                using (var sdr = await cmd.ExecuteReaderAsync())
                {
                    while (sdr.Read())
                    {
                        client.Id = (int)sdr["Id"];
                        client.Civilite = (string)sdr["Civilite"];
                        client.Nom = (string)sdr["Nom"];
                        client.Prenom = (string)sdr["Prenom"];
                        client.Email = (string)sdr["Email"];
                        client.NbReservEnCours = (int)sdr["NbReservationEnCours"];
                    }
                }
            }

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
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
                var user = await _userManager.GetUserAsync(User);

                //Test si email unique
                #region
                Client clientAncien = _context.Client.Where(c => c.Email == user.Email).FirstOrDefault();

                if (clientAncien != null)
                {
                    clientVM.StatusMessage = "Erreur :Adresse Mail deja utilisée pour un compte...";
                    return RedirectToAction("ChangeAccount", "Manage");
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
                    string tel = await _context.Telephone.Where(t => t.Numero == clientVM.TelephoneDom).Select(t => t.Numero).SingleOrDefaultAsync();

                    Telephone telDom = new Telephone()
                    {
                        IdClient = clientVM.id,
                        CodeType = "F",
                        Numero = clientVM.TelephoneDom,
                        Pro = clientVM.ProDom
                    };

                    if (tel == null)
                    {
                        _context.Add(telDom);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        clientVM.StatusMessage = "Numéro de telephone déja existant";
                        return View(clientVM);
                    }
                };
                #endregion

                //Création Telephone Portable
                #region
                if (clientVM.TelephonePort.Length == 10)
                {
                    string tel = await _context.Telephone.Where(t => t.Numero == clientVM.TelephonePort).Select(t => t.Numero).SingleOrDefaultAsync();

                    Telephone telPort = new Telephone()
                    {
                        IdClient = clientVM.id,
                        CodeType = "M",
                        Numero = clientVM.TelephonePort,
                        Pro = clientVM.ProPort
                    };

                    if (tel == null)
                    {
                        _context.Add(telPort);
                        await _context.SaveChangesAsync();
                    }

                    else
                    {
                        clientVM.StatusMessage = "Numéro de telephone déja existant";
                        return View(clientVM);
                    }

                };
                #endregion

                //Si réussi, retourne à l'accueil
                return RedirectToAction(nameof(Index));
            }
            return View(clientVM);
        }



        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit()
        {

            return View();
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreationClientVM clientVM)
        {

            var user = await _userManager.GetUserAsync(User);


            if (ModelState.IsValid)
            {
                try
                {
                    Client clientAncien = _context.Client.Where(c => c.Email == user.Email).FirstOrDefault();

                    Client client = new Client
                    {
                        Civilite = clientVM.Civilite,
                        Nom = clientVM.Nom.ToUpper(),
                        Prenom = clientVM.Prenom[0].ToString().ToUpper() + clientVM.Prenom.Substring(1),
                        Email = user.Email,
                        CarteFidelite = false
                    };

                    clientVM.id = clientAncien.Id;
                    _context.Update(client);
                    await _context.SaveChangesAsync();


                    if (clientVM.AdresseVille != null && clientVM.AdresseRue != null && clientVM.AdresseCodePostal != null)
                    {

                        Adresse Adresse = new Adresse()
                        {
                            IdClient = clientVM.id,
                            Rue = clientVM.AdresseRue,
                            CodePostal = clientVM.AdresseCodePostal,
                            Ville = clientVM.AdresseVille.ToUpper()
                        };
                        _context.Update(Adresse);
                        await _context.SaveChangesAsync();

                    }


                    if (clientVM.TelephoneDom.Length == 10)
                    {
                        string telClient = await _context.Telephone.Where(t => t.IdClient == clientVM.id).Select(t => t.Numero).SingleOrDefaultAsync();
                        string telExist = await _context.Telephone.Where(t => t.Numero == clientVM.TelephoneDom).Select(t => t.Numero).SingleOrDefaultAsync();

                        Telephone telDom = new Telephone()
                        {
                            IdClient = clientVM.id,
                            CodeType = "F",
                            Numero = clientVM.TelephoneDom,
                            Pro = clientVM.ProDom
                        };

                        // Si le client n'avait pas de numéro
                        if (telClient == null)
                        {
                            _context.Telephone.Add(telDom);
                            await _context.SaveChangesAsync();
                        }
                        else if (telExist == null)
                        {
                            _context.Update(telDom);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            clientVM.TelephonePort = "";
                            clientVM.StatusMessage = "Erreur : Numero de telephone Portable déja utilisé..";
                            return RedirectToAction("Create", "Manage", clientVM); // qsdsssssssssssssssssssssssssssssssssssssssss
                        }
                    }


                    if (clientVM.TelephonePort.Length == 10)
                    {
                        string telClient = await _context.Telephone.Where(t => t.IdClient == clientVM.id).Select(t => t.Numero).SingleOrDefaultAsync();
                        string telExist = await _context.Telephone.Where(t => t.Numero == clientVM.TelephonePort).Select(t => t.Numero).SingleOrDefaultAsync();

                        Telephone telPort = new Telephone()
                        {
                            IdClient = clientVM.id,
                            CodeType = "M",
                            Numero = clientVM.TelephonePort,
                            Pro = clientVM.ProPort
                        };

                        // Si le client n'avait pas de numéro
                        if (telClient == null)
                        {
                            _context.Telephone.Add(telPort);
                            await _context.SaveChangesAsync();
                        }
                        else if (telExist == null)
                        {
                            _context.Update(telPort);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            clientVM.TelephonePort = "";
                            clientVM.StatusMessage = "Erreur : Numero de telephone Portable déja utilisé..";
                            return RedirectToAction("Create", "Manage", clientVM); // qsdsssssssssssssssssssssssssssssssssssssssss
                        }
                    }


                }
                finally
                {
                    RedirectToAction("Create", "Manage", clientVM); // qsdsssssssssssssssssssssssssssssssssssssssss

                }
            }
            return RedirectToAction("Create", "Manage", clientVM); // qsdsssssssssssssssssssssssssssssssssssssssss

        }





        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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
    }
}
