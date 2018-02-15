using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandHotel_WebApplication.Models
{
    public class ReservationVM
    {
        public List<Reservation> Reservation { get; set; }
        public List<Chambre> Chambre { get; set; }
        public List<Tarif> Tarif { get; set; }
        public List<TarifChambre> TarifChambre { get; set; }
       
        public TarifChambre ChambreSelectionne { get; set; }

    }
}
