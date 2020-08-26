using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DysFin.Models
{
    /// <summary>
    /// Model danych jednostki kontrolowanej.
    /// </summary>
    public class JednostkaKontrolowana : ModelBaza
    {
        /// <summary>
        /// Symbol.
        /// </summary>
        [Display(Name = "Symbol")]
        [StringLength(8, MinimumLength = 2)]
        public string Symbol { get; set; }

        /// <summary>
        /// Kod procesu głównego realizowanego przez jednostkę kontrolowaną.
        /// </summary>
        [Display(Name = "Proces")]
        public int ProcesId { get; set; }

        [ForeignKey("ProcesId")]
        public SlownikProces Proces { get; set; }

        /// <summary>
        /// Biuro merytorycze jednostki kontrolowanej.
        /// </summary>
        [Display(Name = "Komórka merytoryczna")]
        public int? KomorkaMerytorycznaId { get; set; }

        [ForeignKey("KomorkaMerytorycznaId")]
        public Komorka KomorkaMerytoryczna { get; set; }
    }
}
