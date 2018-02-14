using System;
using System.Collections.Generic;

namespace GrandHotel_WebApplication.Models
{
    public partial class ModePaiement
    {
        public ModePaiement()
        {
            Facture = new HashSet<Facture>();
        }

        public string Code { get; set; }
        public string Libelle { get; set; }

        public ICollection<Facture> Facture { get; set; }
    }
}
