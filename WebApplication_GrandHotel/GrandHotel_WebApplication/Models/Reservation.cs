using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrandHotel_WebApplication.Models
{
    public partial class Reservation 
    {
        public short NumChambre { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Date de début de séjour")]
        [Required(ErrorMessage = "Saississez la date de début de séjour")]
        public DateTime Jour { get; set; }
        public int IdClient { get; set; }
        [Display(Name = "Nombre de personnes")]
        [Required(ErrorMessage = "Saississez le nombre de personnes")]
        public byte NbPersonnes { get; set; }
        [Display(Name = "Heure d'arrivée")]
        [Range(1, 24, ErrorMessage = "entrer un heure entre 1h et 24h")]
        public byte HeureArrivee { get; set; }
        [Display(Name = "Nature du séjour(Travail)")]
        public bool? Travail { get; set; }
        [NotMapped]
        [Display(Name = "Nombre de nuit")]
        [Required(ErrorMessage = "Saississez le nombre de nuit")]
        public int NbNuit { get; set; }

        public Client IdClientNavigation { get; set; }
        public Calendrier JourNavigation { get; set; }
        public Chambre NumChambreNavigation { get; set; }

    }
}