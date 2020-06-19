using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DysFin.Models
{
    /// <summary>
    /// Słownik procesu.
    /// </summary>
    public class SlownikProces : ModelBaza
    {
        /// <summary>
        /// Kod procesu.
        /// </summary>
        [Display(Name = "Kod procesu")]
        public char Kod { get; set; }

        /// <summary>
        /// Kolekcja <see cref="Models.KontrolaProcesy"/>.
        /// </summary>
        public ICollection<KontrolaProcesy> KontrolaProcesy { get; } = new List<KontrolaProcesy>();
    }
}
