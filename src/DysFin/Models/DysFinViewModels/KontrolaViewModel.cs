using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DysFin.Models.DysFinViewModels
{
    public class KontrolaViewModel
    {
        public string Numer { get; set; }

        public string TypKontroliNazwa { get; set; }

        public DateTime DataWpisu { get; set; }

        public string JednostkaKontrolujacaNazwa { get; set; }

        public int JednostkaKontrolujacaId { get; set; }

        public string JednostkaKontrolowanaNazwa { get; set; }

        public int JednostkaKontrolowanaId { get; set; }

        public string KomorkaWiodacaNazwa { get; set; }

        public string KomorkaWiodacaSymbol { get; set; }

        public DateTime DataRozpoczecia { get; set; }

        public DateTime? DataZakonczenia { get; set; }

        public string StatusNazwa { get; set; }
    }
}
