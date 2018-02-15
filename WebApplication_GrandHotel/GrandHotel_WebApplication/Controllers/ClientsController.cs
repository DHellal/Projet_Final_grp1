using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GrandHotel_WebApplication.Data;
using GrandHotel_WebApplication.Models;
using GrandHotel_WebApplication.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity;

namespace GrandHotel_WebApplication.Controllers
{
    public class ClientsController : Controller
    {
        private readonly GrandHotelContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private object someLabel;

        public ClientsController(GrandHotelContext context, UserManager<ApplicationUser> userManager)
        {

            _userManager = userManager;
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index(string lettre)
        {

            var client = await _context.Client
                .Where(a => a.Nom.StartsWith(lettre))
                .Include(c => c.Reservation)
                .OrderBy(a => a.Nom).ToListAsync();
            return View(client);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .SingleOrDefaultAsync(m => m.Id == id);


            //   string req = @"select C.Id, C.Nom, C.Prenom, C.Email,count(R.IdClient) NbReservation
            //                  from Reservation R
            //            left outer join Client C on C.Id = R.IdClient
            //where R.Jour<=GETDATE() And C.Id=@Id
            //group by C.Id, C.Nom, C.Prenom, C.Email
            //order by C.Id";

            //   var Client = new List<Aliment>();
            //   using (var conn = (SqlConnection)_context.Database.GetDbConnection())
            //   {
            //       var cmd = new SqlCommand(req, conn);
            //       cmd.Parameters.Add(new SqlParameter
            //       {
            //           SqlDbType = SqlDbType.Char,
            //           ParameterName = "@codeFamille",
            //           Value = codeFSelect
            //       });
            //       await conn.OpenAsync();

            //       using (var sdr = await cmd.ExecuteReaderAsync())
            //       {
            //           while (sdr.Read())
            //           {
            //               var a = new Aliment();

            //               a.Nom = (string)sdr["Nom"];
            //               a.IdAliment = (int)sdr["IdAliment"];
            //               a.CodeFamille = (string)sdr["CodeFamille"];
            //               a.NbConstituant = (int)sdr["Constituant"];
            //               aliment.Add(a);
            //           }
            //       }
            //   }

            //   vmAliments.Aliments = aliment;
            //   return View(vmAliments);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

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
        public async Task<IActionResult> Create(CreationClientVM clientVM) //[Bind("Civilite,Nom,Prenom,AdresseRue,AdresseCodePostal,AdresseVille,ProDom,ProPort,TelephoneDom,TelephonePort")] 
        {
            if (ModelState.IsValid)
            {

                var user = await _userManager.GetUserAsync(User);
                

                Client client = new Client
                {
                    Civilite = clientVM.Civilite,
                    Nom = clientVM.Nom,
                    Prenom = clientVM.Prenom,
                    Email = user.Email,
                    CarteFidelite = false
                };
                _context.Add(client);
                await _context.SaveChangesAsync();

                int id = _context.Client.OrderBy(c => c.Id).Select(c => c.Id).LastOrDefault();

                if (clientVM.AdresseVille != null && clientVM.AdresseRue != null && clientVM.AdresseCodePostal != null)
                {
                    Adresse Adresse = new Adresse()
                    {
                        IdClient = id,
                        Rue = clientVM.AdresseRue,
                        CodePostal = clientVM.AdresseCodePostal,
                        Ville = clientVM.AdresseVille
                    };
                    _context.Add(Adresse);
                    await _context.SaveChangesAsync();
                }
                if (clientVM.TelephoneDom.Length == 10)
                {
                    Telephone telDom = new Telephone()
                    {
                        IdClient = id,
                        CodeType = "F",
                        Numero = clientVM.TelephoneDom,
                        Pro = clientVM.ProDom

                    };

                    _context.Add(telDom);
                    await _context.SaveChangesAsync();
                };
                if (clientVM.TelephonePort.Length == 10)
                {

                    Telephone telPort = new Telephone()
                    {
                        IdClient = id,
                        CodeType = "M",
                        Numero = clientVM.TelephonePort,
                        Pro = clientVM.ProPort
                    };


                    _context.Add(telPort);
                    await _context.SaveChangesAsync();
                };

                return RedirectToAction(nameof(Index));

            }
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            ModelState.Clear();
            return View(clientVM);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
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
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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
