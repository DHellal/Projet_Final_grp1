using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrandHotel_WebApplication.Models
{
    public partial class Chambre
    {
        public Chambre()
        {
            Reservation = new HashSet<Reservation>();
            TarifChambre = new HashSet<TarifChambre>();
        }
        [Display(Name = "Numero de chambre")]
        public short Numero { get; set; }
        [Display(Name = "Numero d'étage")]
        public byte Etage { get; set; }
        public bool Bain { get; set; }
        public bool? Douche { get; set; }
        public bool? Wc { get; set; }
        [Display(Name = "Nombre de lit")]      
        public byte NbLits { get; set; }
        [Display(Name = "Numero de téléphone")]
        public short? NumTel { get; set; }

        [Display(Name = "Tarif")]
        [NotMapped]
        public decimal Tarifc { get; set; }


        public ICollection<Reservation> Reservation { get; set; }
        public ICollection<TarifChambre> TarifChambre { get; set; }
    }
}
