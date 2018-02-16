using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandHotel_WebApplication.Models
{
    public class ChambreVM
    {
        public List<Chambre> Chambre { get; set; }
        public string StatusChambre { get; set; }
    }
}
