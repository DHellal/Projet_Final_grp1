using GrandHotel_WebApplication.Outil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandHotel_WebApplication.Models
{
    public class FactureVM
    {

        public List<Facture> Factures { get; set; }

        public string AnneeSelected { get; set; }
    }
}
