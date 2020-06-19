using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DysFin.Data;
using DysFin.Models;
using Serilog;

namespace DysFin.Pages.Administracja.KomorkiKontrolowane
{
    /// <summary>
    /// Strona szczegółów komórki kontrolowanej.
    /// </summary>
    public class DetailsModel : PageModel
    {
        private readonly DysFinContext _context;

        public DetailsModel(DysFinContext context)
        {
            _context = context;
        }

        public Komorka Komorka { get; set; }

        /// <summary>
        /// Wyświetla stronę z danymi komórki.
        /// </summary>
        /// <param name="id">Identyfikator komórki kontrolowanej.</param>
        /// <returns>Strona z danymi komórki.</returns>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Komorka = await _context.Komorka.FirstOrDefaultAsync(m => m.Id == id);

            if (Komorka == null)
            {
                return NotFound();
            }

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", Komorka.GetType().Name)
                .ForContext("RecordId", Komorka.Id)
                .Warning("Przegląd komórki {Nazwa}.", Komorka.Nazwa);

            return Page();
        }
    }
}
