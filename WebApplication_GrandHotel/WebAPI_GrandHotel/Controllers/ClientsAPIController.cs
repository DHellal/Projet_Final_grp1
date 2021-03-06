﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebAPI_GrandHotel.Models;

namespace WebAPI_GrandHotel.Controllers
{
    [Produces("application/json")]
    [Route("api/ClientsAPI")]
    public class ClientsAPIController : Controller
    {
        private readonly GrandHotelContext _context;

        public ClientsAPIController(GrandHotelContext context)
        {
            _context = context;
        }

        // GET: api/ClientsAPI
        //Envoie la liste des clients
        [HttpGet]
        public IEnumerable<Client> GetClient()
        {
            return _context.Client;     //.Include(c=>c.Adresse).Include(c=>c.Telephone);
        }

        // GET: api/ClientsAPI/5
        // Envoie le client selon ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClient([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            //Client cli = await _context.Client.Where(m => m.Id == id).Include(a=> a.Adresse).Include(t=> t.Telephone).SingleOrDefaultAsync();

            Client cli = await _context.Client.Where(m => m.Id == id).SingleOrDefaultAsync();

            Adresse addr = await _context.Client.
                        Include(c => c.Adresse).
                        Where(m => m.Id == id).
                        Select(a => new Adresse
                        {
                            Rue = a.Adresse.Rue,
                            Complement = a.Adresse.Complement,
                            CodePostal = a.Adresse.CodePostal,
                            Ville = a.Adresse.Ville
                        }).SingleOrDefaultAsync();

            cli.Adresse = addr;

            List<Telephone> teles = new List<Telephone>();
            int nombreTele = _context.Client.Include(c => c.Telephone).Where(m => m.Id == id).Select(t => t.Telephone).Count();

            for (int i = 0; i < nombreTele; i++)
            {
                Telephone tele = await _context.Client.Include(c => c.Telephone).Where(m => m.Id == id).
                                Select(t => new Telephone
                                {
                                    Numero = t.Telephone[i].Numero,
                                    CodeType = t.Telephone[i].CodeType,
                                    Pro = t.Telephone[i].Pro
                                }).SingleOrDefaultAsync();
                teles.Add(tele);
            }

            cli.Telephone = teles;

            if (cli == null)
            {
                return NotFound();
            }

            return Ok(cli);
        }

        // GET: api/ClientsAPI/5
        //Envoie le client selon NOM
        [HttpGet("FiltreNom/{Nom}")]
        public async Task<IActionResult> GetClientNom([FromRoute] string Nom)
        {
            if (Nom.Length < 3)
            {
                return BadRequest();
            }

            List<Client> clients = await _context.Client.Where(m => m.Nom.Contains(Nom)).ToListAsync();

            if (clients == null)
            {
                return NotFound();
            }

            return Ok(clients);
        }

        // POST: api/ClientsAPI
        // Création du client
        [HttpPost]
        public async Task<IActionResult> PostClient([FromBody] Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Client testEmail = await _context.Client.Where(c => c.Email == client.Email).SingleOrDefaultAsync();

            if (testEmail == null)
            {
                _context.Client.Add(client);
                await _context.SaveChangesAsync();

                int id = await _context.Client.OrderBy(c => c.Id).Select(c => c.Id).LastOrDefaultAsync();

                string idClient = id.ToString();

                return Ok(idClient);
            }
            string mail = "Mail";
            return BadRequest(mail);

        }

        // DELETE: api/ClientsAPI/5
        //Efface le client selon ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var client = await _context.Client.Where(m => m.Id == id).Include(c => c.Adresse).Include(c => c.Telephone).SingleOrDefaultAsync();
            // On empeche la suppression de client avec des telephones ou adresses
            if (client == null || client.Telephone != null || client.Adresse != null)
            {
                return BadRequest();
            }

            _context.Client.Remove(client);
            await _context.SaveChangesAsync();

            return Ok(client);
        }

        private bool ClientExists(int id)
        {
            return _context.Client.Any(e => e.Id == id);
        }
    }
}