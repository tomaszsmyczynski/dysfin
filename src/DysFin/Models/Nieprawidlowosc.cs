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
    /// Model danych nieprawidłowości/zalecenia.
    /// </summary>
    public class Nieprawidlowosc
    {
        /// <summary>
        /// Identyfikator.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Opis.
        /// </summary>
        [Display(Name = "Opis")]
        [Required]
        [StringLength(500, MinimumLength = 5)]
        public string Opis { get; set; }

        /// <summary>
        /// Opis planowanych działań.
        /// </summary>
        [Display(Name = "Opis planowanych działań")]
        [Required]
        [StringLength(500, MinimumLength = 5)]
        public string OpisPlanowanychDzialan { get; set; }

        /// <summary>
        /// Planowana data wykonania.
        /// </summary>
        [Display(Name = "Planowana data wykonania")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Proszę określić planowaną datę usunięcia nieprawidłowości/realizacji zalecenia.")]
        public DateTime PlanowanaData { get; set; }

        /// <summary>
        /// Opis zrealizowanych działań.
        /// </summary>
        [Display(Name = "Opis zrealizowanych działań")]
        public string? OpisZrealizowanychDzialan { get; set; }

        /// <summary>
        /// Data zakończenia działań.
        /// </summary>
        [Display(Name = "Data zakończenia działań")]
        [DataType(DataType.Date)]
        public DateTime? DataZakonczeniaDzialan { get; set; }

        /// <summary>
        /// Identyfikator kontroli.
        /// </summary>
        public int KontrolaId { get; set; }

        public Kontrola Kontrola { get; set; }

        /// <summary>
        /// Indeks.
        /// </summary>
        [NotMapped]
        public int Index { get; set; }
    }
}
