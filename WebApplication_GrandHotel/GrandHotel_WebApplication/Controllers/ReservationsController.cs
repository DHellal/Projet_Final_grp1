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
using GrandHotel_WebApplication.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace GrandHotel_WebApplication.Controllers
{
    // Travail fait par ARMELLE
    //Yiqing a travaillé avec moi pour faire le passage en paramètre des valeurs dans le contrôleurs détails avec les ViewBag.

    public class ReservationsController : Controller
    {
       
        private readonly GrandHotelContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private const string SessionKeyReservationVM = "_ReservationVM";
        
        public ReservationsController(GrandHotelContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
           
        }

       

        //J'affiche le formulaire qui permet à l'utilisateur de saisir les carractéristiques de sa reservation
        public IActionResult Index()
        {

            return View();
        }

        [HttpGet]
        [Route("reservations/{id}/{prix}")]
        public async Task<IActionResult> Details(short id, decimal prix, DateTime jour, byte nbpersonne, bool? travail, int nbnuit, byte heure)
        {
            //je recupère les données passées en parametre avec ces ViewBag pour povoir faire le retour à la page préccedente dans ma vue
            ViewBag.DetailJour = jour.Day;
            ViewBag.DetailMois = jour.Month;
            ViewBag.DetailAnnee = jour.Year;
            ViewBag.DetailNbPersonne = nbpersonne;
            ViewBag.DetailNbnuit = nbnuit;

            //j'affiche le détail de la chambre selectionné par le client
            DateTime date = new DateTime(DateTime.Now.Year, 01, 01);
            ReservationVM chambreVM = new ReservationVM();
            chambreVM.tarifChambre = await _context
                .TarifChambre.Include(m => m.CodeTarifNavigation)
                .Include(m => m.NumChambreNavigation)
                .Where(m => m.NumChambre == id && m.CodeTarifNavigation.DateDebut >= date)
                .FirstOrDefaultAsync();
            chambreVM.tarifChambre.TarifTotal = prix;
            //je crée un guid pour reconnaitre la session de chaque client au cas ou l'on a plusieurs client qui reserve en meme temps
            //Guid g = Guid.NewGuid();
            //var guid = g.ToString();
            //ViewBag.guid = guid;
            //j'enregistre les information de ma reservation dans la session
            var reservation = new Reservation();
            
            reservation.Jour = jour;
            reservation.NbPersonnes = nbpersonne;
            reservation.NumChambre = id;
            reservation.Travail = travail;
            reservation.HeureArrivee = heure;
            reservation.NbNuit = nbnuit;
            reservation.PrixTotal = prix;
            HttpContext.Session.SetObjectAsJson(SessionKeyReservationVM, reservation);
            
            return View(chambreVM);
        }


       
        public async Task<IActionResult> VerifDisponibilite(DateTime Jour, int NbNuit, byte NbPersonnes, byte HeureArrivee, bool? Travail)
        {
            //je stocke les informations de mes parametres dans des ViewBag pour pouvoir les envoyer dans les paramètre de mon action Details
            ViewBag.Nbnuit = NbNuit;
            ViewBag.NbPersonnes = NbPersonnes;
            ViewBag.Jour = Jour.Day;
            ViewBag.Mois = Jour.Month;
            ViewBag.Annee = Jour.Year;
            ViewBag.HeureArrivee = HeureArrivee;
            ViewBag.Travail = Travail;

            ReservationVM chambreVM = new ReservationVM();
            var numeroChambreOccupe = new List<int>();

            //je verifie que la saisie du client dans le formulaire respecte les attributs des propriétés  
            if (ModelState.IsValid)
            {
                //je fais ma requête sql pour recuperer la liste des chambres qui ne correspondent pas au besoin de l'utilisateur
                using (var conn = (SqlConnection)_context.Database.GetDbConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    //je fais une boucle sur le nombre de nuit saisie par l'utilisateur
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
                        //j'incremente le jour saisie par le client jusqu'au nombre de nuit saisie
                        Jour = Jour.AddDays(1);
                    }
                    //liste total des chambre avec le nombre de lits >= au nombre de personnes
                    var chambreTotal = _context.TarifChambre.Include(n=> n.NumChambreNavigation).Where(n=>n.NumChambreNavigation.NbLits>=NbPersonnes);
                    //je deduis dela liste total des chambres les chambres occupées et j'inclus le prix
                    DateTime date = new DateTime(DateTime.Now.Year, 01, 01);
                    chambreVM.TarifChambre = await chambreTotal
                        .Include(t => t.NumChambreNavigation)
                        .Include(t => t.CodeTarifNavigation)
                        .Where(x => !numeroChambreOccupe.Contains(x.NumChambre) && x.CodeTarifNavigation.DateDebut >= date)
                        .ToListAsync();
                }
            }

            if(chambreVM.TarifChambre.Count == 0)
            {
                //j'affiche la vue Indisponible quand il n'y a aucune chambre de disponible
                return View("Indisponible");
            }
            else
            {
                return View(chambreVM);
            }
          
        }

       
        [HttpGet]
        public async Task<IActionResult> Creates()
        {
            
            //je recupere l'email l'email de l'utilisateur
            var user = await _userManager.GetUserAsync(User);
            var email = user.Email;
            //je recupere l'id du client avec l'email
            var id = _context.Client.Where(c => c.Email == email).Select(c => c.Id).FirstOrDefault();
            //je recupere mon objet reservation de ma session
            var reservations = HttpContext.Session.GetObjectFromJson<Reservation>(SessionKeyReservationVM);
            reservations.IdClient = id;
            var duree = reservations.NbNuit;
            //je fais une boucle pour enregistrer la reservion sur la durée du sejour
            for (int i = 0; i < duree; i++)
            {                
                reservations.Jour=reservations.Jour.AddDays(i);
                _context.Add(reservations);
                //j'enregistre la reservation 
                await _context.SaveChangesAsync();
            }
            //je reinitialise la date d'arrivée pour pouvoir afficher la bonne date dans ma vue
            reservations.Jour = reservations.Jour.AddDays(-(duree-1));
            //je vide ma session pour q'un client puisse accéder
            HttpContext.Session.Remove(SessionKeyReservationVM);
            return View(reservations);
            
        }

        public async Task<IActionResult> VerifConnexion()
        {
            //on verifie que le client est connecté
            var user = await _userManager.GetUserAsync(User);
            //si le client n'est connecté on le redirige vers la page ou il peut créer son compte ou se connecter
            if (user==null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                //si l'id du client existe déjà en base de donnée et cela veut dire qu'il est dejà client et on le redirige vers l'action qui crée la réservation
                var idClientExist = _context.Client.Where(c => c.Email == user.Email).Select(i=>i.Id).FirstOrDefaultAsync();
                if (idClientExist != null)
                {
                    return RedirectToAction("Creates");
                }
                //si l'id du client n'existe pas on lui permet de créer son compte
                else
                {
                    return RedirectToAction("Create", "Client");
                }
            }
        }
    }
}
   
