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
        public DateTime Jour { get; set; }
        public int IdClient { get; set; }
        [Display(Name = "Nombre de personnes")]
        public byte NbPersonnes { get; set; }
        [Display(Name = "Heure d'arrivée")]
        public byte HeureArrivee { get; set; }
        [Display(Name = "Nature du séjour(Travail)")]
        public bool? Travail { get; set; }
        [NotMapped]
        [Display(Name = "Nombre de nuit")]
        public int NbNuit { get; set; }
        public Client IdClientNavigation { get; set; }
        public Calendrier JourNavigation { get; set; }
        public Chambre NumChambreNavigation { get; set; }
    }
}
