using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DysFin.Models
{
    public class ModelBaza
    {
        /// <summary>
        /// Identyfikator.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nazwa.
        /// </summary>
        [Display(Name = "Nazwa")]
        [Required()]
        [StringLength(500, MinimumLength = 2)]
        public string Nazwa { get; set; }
    }
}
