using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrandHotel_WebApplication.Models
{
    public partial class Client
    {
        public Client()
        {
            Facture = new HashSet<Facture>();
            Reservation = new HashSet<Reservation>();
            Telephone = new HashSet<Telephone>();
        }

        public int Id { get; set; }
        public string Civilite { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public bool CarteFidelite { get; set; }
        public string Societe { get; set; }

        [Display(Name = "Reservation en cours")]
        [NotMapped]
        public int NbReservEnCours { get; set; }

        [Display(Name = "Total de reservations")]
        [NotMapped]
        public int NbReservation { get; set; }


        public Adresse Adresse { get; set; }
        public ICollection<Facture> Facture { get; set; }
        [Display(Name = "Reservations effectuées")]
        public ICollection<Reservation> Reservation { get; set; }
        public ICollection<Telephone> Telephone { get; set; }
    }
}
