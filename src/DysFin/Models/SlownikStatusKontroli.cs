using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DysFin.Models
{
    /// <summary>
    /// Słownik statusów kontroli.
    /// </summary>
    public class SlownikStatusKontroli : ModelBaza
    {
        /// <summary>
        /// Symbol statusu.
        /// </summary>
        [Display(Name = "Symbol")]
        public string Symbol { get; set; }
    }
}
