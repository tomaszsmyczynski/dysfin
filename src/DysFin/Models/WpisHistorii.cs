using DysFin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DysFin.Web.Models
{
    public class WpisHistorii
    {
        public DateTime Data { get; set; }

        public string Autor { get; set; }

        public string Wiadomosc { get; set; }

        public Kontrola KontrolaPrzed { get; set; }

        public Kontrola KontrolaPo { get; set; }

        public Nieprawidlowosc NieprawidlowoscPrzed { get; set; }

        public Nieprawidlowosc NieprawidlowoscPo { get; set; }
    }
}
