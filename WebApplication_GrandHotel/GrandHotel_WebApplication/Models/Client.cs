using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrandHotel_WebApplication.Models
{
    public partial class Client
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Requis")]
        public string Civilite { get; set; }
        [Required(ErrorMessage = "Requis")]
        public string Nom { get; set; }
        [Required(ErrorMessage = "Requis")]
        public string Prenom { get; set; }
        
        public string Email { get; set; }
        [Display(Name = "Carte de fidélité")]
        public bool CarteFidelite { get; set; }
        public string Societe { get; set; }

        [Display(Name = "Reservations Totales")]
        [NotMapped]
        public int NbReservation { get; set; }

        [Display(Name = "Reservations en cours")]
        [NotMapped]
        public int NbReservationEnCours { get; set; }

        public Adresse Adresse { get; set; }
        public List<Facture> Facture { get; set; }
        [Display(Name = "Reservations effectuées")]
        public List<Reservation> Reservation { get; set; }

        public List<Telephone> Telephone { get; set; }
    }
}
