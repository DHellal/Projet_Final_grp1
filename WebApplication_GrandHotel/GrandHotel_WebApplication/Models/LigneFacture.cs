using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GrandHotel_WebApplication.Models
{
    public partial class LigneFacture
    {
        public int IdFacture { get; set; }
        [Display(Name ="N°")]
        public int NumLigne { get; set; }
        [Display(Name = "Quantité")]
        public short Quantite { get; set; }
        [Display(Name = "Montant HT")]
        public decimal MontantHt { get; set; }
        [Display(Name = "Taux Tva")]
        public decimal TauxTva { get; set; }
        [Display(Name = "Taux de Réduction")]
        public decimal TauxReduction { get; set; }

        public Facture IdFactureNavigation { get; set; }
    }
}
