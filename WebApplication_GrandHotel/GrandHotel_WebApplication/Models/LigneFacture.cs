using System;
using System.Collections.Generic;

namespace GrandHotel_WebApplication.Models
{
    public partial class LigneFacture
    {
        public int IdFacture { get; set; }
        public int NumLigne { get; set; }
        public short Quantite { get; set; }
        public decimal MontantHt { get; set; }
        public decimal TauxTva { get; set; }
        public decimal TauxReduction { get; set; }

        public Facture IdFactureNavigation { get; set; }
    }
}
