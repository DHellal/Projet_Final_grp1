﻿using System;
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

        [Display(Name = "Id Facture")]
        public int Id { get; set; }
        public int IdClient { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Date de Facture")]
        public DateTime DateFacture { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Date de Paiement")]
        public DateTime? DatePaiement { get; set; }
        [Display(Name = "Mode de Paiement")]
        public string CodeModePaiement { get; set; }

        public ModePaiement CodeModePaiementNavigation { get; set; }
        public Client IdClientNavigation { get; set; }
        public ICollection<LigneFacture> LigneFacture { get; set; }

    }
}
