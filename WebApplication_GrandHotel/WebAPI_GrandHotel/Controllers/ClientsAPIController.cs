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
        [HttpGet]
        public IEnumerable<Client> GetClient()
        {
            return _context.Client;
        }

        // GET: api/ClientsAPI/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClient([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var client = await _context.Client.SingleOrDefaultAsync(m => m.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        // GET: api/ClientsAPI/5
        [HttpGet("FiltreNom/{Nom}")]
        public async Task<IActionResult> GetClientNom([FromRoute] string Nom)
        {
            if (Nom.Length <3)
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

        //// PUT: api/ClientsAPI/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutClient([FromRoute] int id, [FromBody] Client client)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != client.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(client).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ClientExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}



        // POST: api/ClientsAPI
        [HttpPost]
        public async Task<IActionResult> PostClient([FromBody] Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Client testEmail = await _context.Client.Where(c => c.Email == client.Email).SingleOrDefaultAsync();

            if(testEmail == null)
            {
            _context.Client.Add(client);
            await _context.SaveChangesAsync();

             int id =  await _context.Client.OrderBy(c=> c.Id).Select(c=> c.Id).LastOrDefaultAsync();

                string idClient = "Client créé à l'id" +id.ToString();

            return Ok(idClient);
            }
            object mail = "Email deja prise...";
            return BadRequest( error: mail);

        }

        // DELETE: api/ClientsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var client = await _context.Client.SingleOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
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