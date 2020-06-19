using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DysFin.Data;
using DysFin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace DysFin.Pages.Kontrole
{
    /// <summary>
    /// Strona dodawania nieprawidłowości do kontroli.
    /// </summary>
    public class NieprawidlowosciModel : PageModel
    {
        private readonly DysFinContext _context;

        public NieprawidlowosciModel(DysFinContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Wyświetla formularz dodania nowej nieprawidłowości/zalecenia.
        /// </summary>
        /// <param name="id">Identyfikator kontroli.</param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kontrola = await _context.Kontrola
                .Include(k => k.Status)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (kontrola == null)
            {
                return NotFound();
            }

            if (kontrola.StatusId != 41)
            {
                ViewData["Title"] = "Kontrola zewnętrzna - nowa nieprawidłowość/zalecenie";
                ViewData["KontrolaId"] = id;

                return Page();
            }
            else
            {
                return RedirectToPage("./Index", new { closed = "Aby dodać nieprawidłowość/zalecenie kontrola nie może być w statusie Zamknięta." });
            }
        }

        /// <summary>
        /// Usuwa wybraną nieprawidłowość z kontroli.
        /// </summary>
        /// <param name="id">Identyfikator nieprawidłowości.</param>
        /// <param name="kontrolaId">Identyfikator kontroli.</param>
        /// <returns>Przekierowania do strony edycji kontroli.</returns>
        public async Task<IActionResult> OnGetDelete(int? id, int kontrolaId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kontrola = await _context.Kontrola
                .Include(k => k.Status)
                .Include(k => k.Nieprawidlowosci)
                .FirstOrDefaultAsync(m => m.Id == kontrolaId);

            if (kontrola.StatusId == 43 && kontrola.Nieprawidlowosci.Count == 1)
            {
                return RedirectToPage("./Index", new { closed = "Kontrola w statusie Działania naprawcze musi mieć nieprawidłowości/zalecenia." });
            }

            var nieprawidlowosc = await _context.Nieprawidlowosc.FindAsync(id);

            if (nieprawidlowosc != null)
            {
                _context.Nieprawidlowosc.Remove(nieprawidlowosc);
                await _context.SaveChangesAsync();
            }

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", nameof(Nieprawidlowosc))
                .ForContext("RecordId", nieprawidlowosc.Id)
                .ForContext("MainRecordId", kontrolaId)
                .Warning("Usunięcie nieprawidłowości.");

            return RedirectToPage("./Edit", new { id = kontrolaId });
        }

        /// <summary>
        /// Identyfikator kontroli.
        /// </summary>
        [BindProperty]
        public int KontrolaId { get; set; }

        /// <summary>
        /// Pozycja <see cref="Nieprawidlowosc"/>.
        /// </summary>
        [BindProperty]
        public Nieprawidlowosc Nieprawidlowosc { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Dodaje nieprawidłowość i przypisuje ją do kontroli.
        /// </summary>
        /// <returns>Przekierowanie do listy kontroli.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["KontrolaId"] = KontrolaId;

                return Page();
            }

            Nieprawidlowosc.KontrolaId = KontrolaId;

            _context.Nieprawidlowosc.Add(Nieprawidlowosc);
            await _context.SaveChangesAsync();

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", nameof(Kontrola))
                .ForContext("RecordId", Nieprawidlowosc.Id)
                .ForContext("MainRecordId", KontrolaId)
                .Warning("Dodanie nieprawidłowości.");

            var kontrola = await _context.Kontrola.FirstOrDefaultAsync(m => m.Id == KontrolaId);

            if (kontrola.StatusId != 43)
            {
                kontrola.StatusId = 43;

                _context.Attach(kontrola).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                Log
                    .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                    .ForContext("Table", kontrola.GetType().Name)
                    .ForContext("RecordId", kontrola.Id)
                    .ForContext("MainRecordId", kontrola.Id)
                    .Warning("Zmiana statusu kontroli {Numer}.", kontrola.Numer);
            }

            return RedirectToPage("./Nieprawidlowosci", new { id = kontrola.Id });
        }
    }
}
