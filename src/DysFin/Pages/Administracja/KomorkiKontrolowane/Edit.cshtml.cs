using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DysFin.Data;
using DysFin.Models;
using Serilog;

namespace DysFin.Pages.Administracja.KomorkiKontrolowane
{
    /// <summary>
    /// Strona edycji danych komórki kontrolowanej.
    /// </summary>
    public class EditModel : PageModel
    {
        private readonly DysFinContext _context;

        public EditModel(DysFinContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Komorka Komorka { get; set; }

        /// <summary>
        /// Wyświetla formularz edycji danych komórki kontrolowanej.
        /// </summary>
        /// <param name="id">Identyfikator komórki kontrolowanej.</param>
        /// <returns>Strona z formularzem.</returns>
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
                .Warning("Otwarcie edycji komórki {Nazwa}.", Komorka.Nazwa);

            return Page();
        }

        /// <summary>
        /// Przesyła model danych <see cref="Models.Komorka"/> do zaktualizowania w aplikacji.
        /// </summary>
        /// <returns>Przekierowanie do listy komórek kontrolowanych.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Komorka).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KomorkaExists(Komorka.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", Komorka.GetType().Name)
                .ForContext("RecordId", Komorka.Id)
                .Warning("Zapisanie edycji komórki {Nazwa}.", Komorka.Nazwa);

            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Sprawdza czy komórka kontrolowana istnieje.
        /// </summary>
        /// <param name="id">Identyfikator komórki kontrolowanej.</param>
        /// <returns>Prawda/Fałsz.</returns>
        private bool KomorkaExists(int id)
        {
            return _context.Komorka.Any(e => e.Id == id);
        }
    }
}
