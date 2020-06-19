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

namespace DysFin.Pages.Administracja.JednostkiKontrolujace
{
    /// <summary>
    /// Strona edycji danych jednostki kontrolującej.
    /// </summary>
    public class EditModel : PageModel
    {
        private readonly DysFinContext _context;

        public EditModel(DysFinContext context)
        {
            _context = context;
        }

        [BindProperty]
        public JednostkaKontrolujaca JednostkaKontrolujaca { get; set; }

        /// <summary>
        /// Wyświetla formularz edycji danych jednostki kontrolującej.
        /// </summary>
        /// <param name="id">Identyfikator jednostki kontrolującej.</param>
        /// <returns>Strona z formularzem.</returns>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            JednostkaKontrolujaca = await _context.JednostkaKontrolujaca.FirstOrDefaultAsync(m => m.Id == id);

            if (JednostkaKontrolujaca == null)
            {
                return NotFound();
            }

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", JednostkaKontrolujaca.GetType().Name)
                .ForContext("RecordId", JednostkaKontrolujaca.Id)
                .Warning("Otwarcie edycji jednostki kontrolującej {Nazwa}.", JednostkaKontrolujaca.Nazwa);

            return Page();
        }

        /// <summary>
        /// Przesyła model danych <see cref="Models.JednostkaKontrolujaca"/> do zaktualizowania w aplikacji.
        /// </summary>
        /// <returns>Przekierowanie do listy jednostek kontrolujących.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(JednostkaKontrolujaca).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JednostkaKontrolujacaExists(JednostkaKontrolujaca.Id))
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
                .ForContext("Table", JednostkaKontrolujaca.GetType().Name)
                .ForContext("RecordId", JednostkaKontrolujaca.Id)
                .Warning("Zapisanie edycji jednostki kontrolującej {Nazwa}.", JednostkaKontrolujaca.Nazwa);

            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Sprawdza czy jednostka kontrolująca istnieje.
        /// </summary>
        /// <param name="id">Identyfikator jednostki kontrolującej.</param>
        /// <returns>Prawda/Fałsz.</returns>
        private bool JednostkaKontrolujacaExists(int id)
        {
            return _context.JednostkaKontrolujaca.Any(e => e.Id == id);
        }
    }
}
