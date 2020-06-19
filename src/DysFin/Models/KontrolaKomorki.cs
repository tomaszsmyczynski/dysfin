using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DysFin.Models
{
    /// <summary>
    /// Model danych komórki współpracującej w kontroli.
    /// </summary>
    public class KontrolaKomorki
    {
        /// <summary>
        /// Identyfikator kontroli.
        /// </summary>
        public int KontrolaId { get; set; }

        public Kontrola Kontrola { get; set; }

        /// <summary>
        /// Identyfikator komórki.
        /// </summary>
        public int KomorkaId { get; set; }

        public Komorka Komorka { get; set; }

        /// <summary>
        /// Indeks.
        /// </summary>
        [NotMapped]
        public int Index { get; set; }
    }
}
