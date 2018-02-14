using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrandHotel_WebApplication.Models
{
    public partial class Facture
    {
        public Facture()
        {
            LigneFacture = new HashSet<LigneFacture>();
        }

        [Display(Name="IdFacture")]
        public int Id { get; set; }
        public int IdClient { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateFacture { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DatePaiement { get; set; }
        public string CodeModePaiement { get; set; }

        public ModePaiement CodeModePaiementNavigation { get; set; }
        public Client IdClientNavigation { get; set; }
        public ICollection<LigneFacture> LigneFacture { get; set; }

        [NotMapped]
        public string AnneeEnCour { get; set; }
    }
}
