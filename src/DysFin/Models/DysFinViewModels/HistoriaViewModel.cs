using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DysFin.Web.Models.DysFinViewModels
{
    public class HistoriaViewModel
    {
        public string Kontrola { get; set; }

        public List<WpisHistorii> Wpisy = new List<WpisHistorii>();
    }
}
