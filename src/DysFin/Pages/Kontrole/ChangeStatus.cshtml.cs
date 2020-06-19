using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DysFin.Data;
using DysFin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DysFin.Web.Pages.Kontrole
{
    public class ChangeStatusModel : PageModel
    {
        private readonly DysFinContext _context;

        public ChangeStatusModel(DysFinContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int KontrolaId { get; set; }

        [BindProperty]
        public int NewStatusId { get; set; }

        [BindProperty]
        public string UwagiKW { get; set; }

        /// <summary>
        /// Wyświetlenie strony do zmiany statusu kontroli.
        /// </summary>
        /// <param name="id">Identyfikator kontroli.</param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kontrola = await _context.Kontrola.FirstOrDefaultAsync(m => m.Id == id);

            if (kontrola == null)
            {
                return NotFound();
            }

            KontrolaId = kontrola.Id;
            UwagiKW = kontrola.UwagiKW;

            ViewData["StatusId"] = new SelectList(_context.SlownikStatusKontroli.OrderBy(t => t.Nazwa), "Id", "Nazwa").Prepend(new SelectListItem("Wybierz", "0"));

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", kontrola.GetType().Name)
                .ForContext("RecordId", kontrola.Id)
                .Warning("Otwarcie edycji statusu kontroli {Numer}.", kontrola.Numer);

            return Page();
        }

        /// <summary>
        /// Metoda do zmiany statusu kontroli.
        /// </summary>
        /// <param name="kontrolaId">Identyfikator kontroli.</param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var kontrola = await _context.Kontrola.FirstOrDefaultAsync(m => m.Id == KontrolaId);

            kontrola.StatusId = NewStatusId;
            kontrola.UwagiKW = UwagiKW;

            _context.Attach(kontrola).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StatusExists(NewStatusId))
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
                .ForContext("Table", kontrola.GetType().Name)
                .ForContext("RecordId", kontrola.Id)
                .ForContext("MainRecordId", kontrola.Id)
                .Warning("Zmiana statusu kontroli {Numer}.", kontrola.Numer);

            return RedirectToPage("./Details", new { id = kontrola.Id });
        }

        /// <summary>
        /// Sprawdza czy status kontroli istnieje.
        /// </summary>
        /// <param name="id">Identyfikator statusu kontroli.</param>
        /// <returns>Prawda/Fałsz.</returns>
        private bool StatusExists(int id)
        {
            return _context.SlownikStatusKontroli.Any(e => e.Id == id);
        }
    }
}