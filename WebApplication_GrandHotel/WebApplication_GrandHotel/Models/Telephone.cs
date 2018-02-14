using System;
using System.Collections.Generic;

namespace WebApplication_GrandHotel.Models
{
    public partial class Telephone
    {
        public string Numero { get; set; }
        public int IdClient { get; set; }
        public string CodeType { get; set; }
        public bool Pro { get; set; }

        public Client IdClientNavigation { get; set; }
    }
}
