using System;
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
            return _context.Client.Include(c=>c.Adresse).Include(c=>c.Telephone);
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

            var client = await _context.Client.Where(m => m.Id == id).Include(c => c.Adresse).Include(c => c.Telephone).SingleOrDefaultAsync();

            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
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

            List<Client> clients = await _context.Client.Where(m => m.Nom.Contains(Nom)).Include(c => c.Adresse).Include(c => c.Telephone).ToListAsync();

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