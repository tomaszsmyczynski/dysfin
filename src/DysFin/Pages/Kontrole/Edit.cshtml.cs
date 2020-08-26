using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DysFin.Data;
using DysFin.Models;
using Serilog;

namespace DysFin.Pages.Kontrole
{
    /// <summary>
    /// Strona edycji danych kontroli zewnętrznej.
    /// </summary>
    public class EditModel : PageModel
    {
        private readonly DysFinContext _context;

        public EditModel(DysFinContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Kontrola Kontrola { get; set; }

        /// <summary>
        /// Wyświetla formularz edycji kontroli.
        /// </summary>
        /// <param name="id">Identyfikator kontroli.</param>
        /// <returns>Strona z formularzem.</returns>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Kontrola = await _context.Kontrola
                .Include(k => k.JednostkaKontrolowana)
                .Include(k => k.JednostkaKontrolujaca)
                .Include(k => k.KomorkaWiodaca)
                .Include(k => k.KomorkiUczestniczace).ThenInclude(k => k.Komorka)
                .Include(k => k.Nieprawidlowosci)
                .Include(k => k.Status)
                .Include(k => k.TypKontroli)
                .Include(k => k.Zalaczniki)
                .Include(k => k.Procesy).ThenInclude(k => k.Proces)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Kontrola == null)
            {
                return NotFound();
            }

            if (Kontrola.StatusId == 40 || Kontrola.StatusId == 41)
            {
                return RedirectToPage("./Index");
            }

            if (Kontrola.StatusId == 17 || Kontrola.StatusId == 43)
            {
                ViewData["StatusId"] = new SelectList(_context.SlownikStatusKontroli.Where(s => s.Id == 17 || s.Id == 40 || s.Id == 43), "Id", "Nazwa");
            }
            else
            {
                ViewData["StatusId"] = new SelectList(_context.SlownikStatusKontroli.Where(s => s.Id == 40 || s.Id == 42 || s.Id == 43), "Id", "Nazwa");
            }

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", Kontrola.GetType().Name)
                .ForContext("RecordId", Kontrola.Id)
                .Warning("Otwarcie edycji kontroli {Numer}.", Kontrola.Numer);

            return Page();
        }

        /// <summary>
        /// Przesyła model danych <see cref="Models.Kontrola"/> do zaktualizowania w aplikacji.
        /// </summary>
        /// <returns>Przekierowanie do szczegółów kontroli.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            var old = _context.Kontrola.Include(k => k.Nieprawidlowosci).AsNoTracking().FirstOrDefault(k => k.Id == Kontrola.Id);

            if (Kontrola.StatusId == 43 && old.Nieprawidlowosci.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Zanim zmienisz status kontroli na Działania naprawcze dodaj najpierw nieprawidłowości/zalecenia.");
            }

            if (!ModelState.IsValid)
            {
                Kontrola = await _context.Kontrola
                .Include(k => k.JednostkaKontrolowana)
                .Include(k => k.JednostkaKontrolujaca)
                .Include(k => k.KomorkaWiodaca)
                .Include(k => k.KomorkiUczestniczace).ThenInclude(k => k.Komorka)
                .Include(k => k.Nieprawidlowosci)
                .Include(k => k.Status)
                .Include(k => k.TypKontroli)
                .Include(k => k.Zalaczniki)
                .Include(k => k.Procesy).ThenInclude(k => k.Proces)
                .FirstOrDefaultAsync(m => m.Id == Kontrola.Id);

                if (Kontrola.StatusId == 17 || Kontrola.StatusId == 43)
                {
                    ViewData["StatusId"] = new SelectList(_context.SlownikStatusKontroli.Where(s => s.Id == 17 || s.Id == 40 || s.Id == 43), "Id", "Nazwa");
                }
                else
                {
                    ViewData["StatusId"] = new SelectList(_context.SlownikStatusKontroli.Where(s => s.Id == 40 || s.Id == 42 || s.Id == 43), "Id", "Nazwa");
                }

                return Page();
            }

            _context.Attach(Kontrola).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KontrolaExists(Kontrola.Id))
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
                .ForContext("Table", Kontrola.GetType().Name)
                .ForContext("RecordId", Kontrola.Id)
                .ForContext("MainRecordId", Kontrola.Id)
                .ForContext("Old", JsonSerializer.Serialize(old))
                .ForContext("New", JsonSerializer.Serialize(Kontrola))
                .Warning("Zapisanie edycji kontroli {Numer}.", Kontrola.Numer);

            return RedirectToPage("./Details", new { id = Kontrola.Id });
        }

        /// <summary>
        /// Sprawdza czy kontrola istnieje.
        /// </summary>
        /// <param name="id">Identyfikator kontroli.</param>
        /// <returns>Prawda/Fałsz.</returns>
        private bool KontrolaExists(int id)
        {
            return _context.Kontrola.Any(e => e.Id == id);
        }
    }
}
