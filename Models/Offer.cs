using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GMCOffer.Models
{
    public class Offer
    {
        public int ccgsku { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int boh { get; set; }
        public bool active { get; set; }
        public int sort { get; set; }

        public Offer() { }
       
    }
}