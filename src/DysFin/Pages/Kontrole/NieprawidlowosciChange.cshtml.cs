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
using System.Text.Json;

namespace DysFin.Pages.Kontrole
{
    /// <summary>
    /// Strona edycji nieprawidłowości.
    /// </summary>
    public class NieprawidlowosciChangeModel : PageModel
    {
        private readonly DysFinContext _context;

        public NieprawidlowosciChangeModel(DysFinContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Nieprawidlowosc Nieprawidlowosc { get; set; }

        /// <summary>
        /// Wyświetla stronę edycji nieprawidłowości.
        /// </summary>
        /// <param name="id">Identyfikator.</param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Nieprawidlowosc = await _context.Nieprawidlowosc.FirstOrDefaultAsync(m => m.Id == id);

            if (Nieprawidlowosc == null)
            {
                return NotFound();
            }

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", Nieprawidlowosc.GetType().Name)
                .ForContext("RecordId", Nieprawidlowosc.Id)
                .Warning("Otwarcie edycji nieprawidłowości.");

            return Page();
        }

        /// <summary>
        /// Przesyła model danych <see cref="Models.Nieprawidlowosc"/> do zaktualizowania w aplikacji.
        /// </summary>
        /// <returns>Przekierowanie do strony edycji kontroli.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var old = _context.Nieprawidlowosc.AsNoTracking().FirstOrDefault(n => n.Id == Nieprawidlowosc.Id);

            _context.Attach(Nieprawidlowosc).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NieprawidlowoscExists(Nieprawidlowosc.Id))
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
                .ForContext("Table", Nieprawidlowosc.GetType().Name)
                .ForContext("RecordId", Nieprawidlowosc.Id)
                .ForContext("MainRecordId", Nieprawidlowosc.KontrolaId)
                .ForContext("Old", JsonSerializer.Serialize(old))
                .ForContext("New", JsonSerializer.Serialize(Nieprawidlowosc))
                .Warning("Zapisanie edycji nieprawidłowości.");

            return RedirectToPage("./Edit", new { id = Nieprawidlowosc.KontrolaId });
        }

        /// <summary>
        /// Sprawdza czy nieprawidłowość/zalecenie istnieje.
        /// </summary>
        /// <param name="id">Identyfikator.</param>
        /// <returns>Prawda/Fałsz.</returns>
        private bool NieprawidlowoscExists(int id)
        {
            return _context.Nieprawidlowosc.Any(e => e.Id == id);
        }
    }
}
