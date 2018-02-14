using System;
using System.Collections.Generic;

namespace GrandHotel_WebApplication.Models
{
    public partial class Facture
    {
        public Facture()
        {
            LigneFacture = new HashSet<LigneFacture>();
        }

        public int Id { get; set; }
        public int IdClient { get; set; }
        public DateTime DateFacture { get; set; }
        public DateTime? DatePaiement { get; set; }
        public string CodeModePaiement { get; set; }

        public ModePaiement CodeModePaiementNavigation { get; set; }
        public Client IdClientNavigation { get; set; }
        public ICollection<LigneFacture> LigneFacture { get; set; }
    }
}
