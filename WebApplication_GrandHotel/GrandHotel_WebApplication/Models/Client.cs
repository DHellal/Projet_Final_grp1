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
        [Required(ErrorMessage = "Requis")]
        public string Civilite { get; set; }
        [Required(ErrorMessage = "Requis")]
        public string Nom { get; set; }
        [Required(ErrorMessage = "Requis")]
        public string Prenom { get; set; }
        
        public string Email { get; set; }
        public bool CarteFidelite { get; set; }
        public string Societe { get; set; }

        [Display(Name = "Reservations Totales")]
        [NotMapped]
        public int NbReservation { get; set; }

        [Display(Name = "Reservations en cours")]
        [NotMapped]
        public int NbReservationEnCours { get; set; }

        public Adresse Adresse { get; set; }
        public ICollection<Facture> Facture { get; set; }
        [Display(Name = "Reservations effectuées")]
        public ICollection<Reservation> Reservation { get; set; }
        [Phone]
        public ICollection<Telephone> Telephone { get; set; }
    }
}
