using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

#nullable enable
namespace DysFin.Models
{
    /// <summary>
    /// Model danych załącznika.
    /// </summary>
    public class Zalacznik : ModelBaza
    {
        /// <summary>
        /// Identyfikator kontroli.
        /// </summary>
        public int KontrolaId { get; set; }

        public Kontrola Kontrola { get; set; } = default!;

        /// <summary>
        /// Zawartość.
        /// </summary>
        public byte[] Zawartosc { get; set; } = default!;

        /// <summary>
        /// Opis.
        /// </summary>
        public string? Opis { get; set; }

        /// <summary>
        /// Rozmiar pliku.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long Rozmiar { get; set; }

        /// <summary>
        /// Typ zawartości.
        /// </summary>
        public string TypZawartosci { get; set; } = default!;

        /// <summary>
        /// Data.
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime Data { get; set; }
    }
}
