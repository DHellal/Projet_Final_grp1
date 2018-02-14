using GrandHotel_WebApplication.Models;
using System;
using System.Collections.Generic;

namespace GrandHotel_WebApplication
{
    public partial class Adresse
    {
        public int IdClient { get; set; }
        public string Rue { get; set; }
        public string Complement { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }

        public Client IdClientNavigation { get; set; }
    }
}
