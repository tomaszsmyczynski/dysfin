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

namespace DysFin.Pages.Administracja.Uzytkownicy
{
    /// <summary>
    /// Strona edycji danych użytkownika.
    /// </summary>
    public class EditModel : PageModel
    {
        private readonly DysFinContext _context;

        public EditModel(DysFinContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Uzytkownik Uzytkownik { get; set; }

        /// <summary>
        /// Wyświetla formularz edycji danych użytkownika.
        /// </summary>
        /// <param name="id">Identyfikator użytkownika.</param>
        /// <returns>Strona z formularzem.</returns>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Uzytkownik = await _context.Uzytkownik
                //.Include(u => u.Jednostka)
                .Include(u => u.Komorka)
                .Include(u => u.PoziomUzytkownika).FirstOrDefaultAsync(m => m.Id == id);

            if (Uzytkownik == null)
            {
                return NotFound();
            }

            ViewData["JednostkaId"] = new SelectList(_context.JednostkaKontrolowana, "Id", "Nazwa");
            ViewData["KomorkaId"] = new SelectList(_context.Komorka.OrderBy(k => k.Nazwa), "Id", "Nazwa");
            ViewData["PoziomUzytkownikaId"] = new SelectList(_context.SlownikPoziomUzytkownika, "Id", "Nazwa");

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", Uzytkownik.GetType().Name)
                .ForContext("RecordId", Uzytkownik.Id)
                .Warning("Otwarcie edycji użytkownika {Login}.", Uzytkownik.Login);

            return Page();
        }

        /// <summary>
        /// Przesyła model danych <see cref="Models.Uzytkownik"/> do zaktualizowania w aplikacji.
        /// </summary>
        /// <returns>Przekierowanie do listy użytkowników.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["JednostkaId"] = new SelectList(_context.JednostkaKontrolowana, "Id", "Nazwa");
                ViewData["KomorkaId"] = new SelectList(_context.Komorka.OrderBy(k => k.Nazwa), "Id", "Nazwa");
                ViewData["PoziomUzytkownikaId"] = new SelectList(_context.SlownikPoziomUzytkownika, "Id", "Nazwa");

                return Page();
            }

            _context.Attach(Uzytkownik).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UzytkownikExists(Uzytkownik.Id))
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
                .ForContext("Table", Uzytkownik.GetType().Name)
                .ForContext("RecordId", Uzytkownik.Id)
                .Warning("Zapisanie edycji użytkownika {Login}.", Uzytkownik.Login);

            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Sprawdza istnienie użytkownika o podanym identyfikatorze.
        /// </summary>
        /// <param name="id">Identyfikator użytkownika.</param>
        /// <returns>Prawda/Fałsz.</returns>
        private bool UzytkownikExists(int id)
        {
            return _context.Uzytkownik.Any(e => e.Id == id);
        }
    }
}
