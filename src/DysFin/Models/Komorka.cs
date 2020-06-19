using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DysFin.Models
{
    /// <summary>
    /// Model danych komórki organizacyjnej.
    /// </summary>
    public class Komorka : ModelBaza
    {
        /// <summary>
        /// Symbol komórki.
        /// </summary>
        [Display(Name = "Symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// Kod procesu głównego realizowanego przez jednostkę kontrolowaną.
        /// </summary>
        [Display(Name = "Kod procesu głównego")]
        public int? ProcesId { get; set; }

        [ForeignKey("ProcesId")]
        public SlownikProces Proces { get; set; }


        /// <summary>
        /// Kolekcja <see cref="Models.KontrolaKomorki"/>.
        /// </summary>
        public ICollection<KontrolaKomorki> KontrolaKomorki { get; } = new List<KontrolaKomorki>();
    }
}
