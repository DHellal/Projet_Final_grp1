using System;
using System.Collections.Generic;

namespace WebAPI_GrandHotel.Models
{
    public partial class Client
    {
        public Client()
        {
            Adresse = new Adresse();
            Telephone = new List<Telephone>();
        }

        public int Id { get; set; }
        public string Civilite { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public bool CarteFidelite { get; set; }
        public string Societe { get; set; }

        public Adresse Adresse { get; set; }
        public List<Telephone> Telephone { get; set; }
    }
}
