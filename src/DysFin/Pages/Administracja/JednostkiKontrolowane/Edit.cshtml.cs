﻿using System;
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

namespace DysFin.Pages.Administracja.JednostkiKontrolowane
{
    /// <summary>
    /// Strona edycji danych jednostki kontrolowanej.
    /// </summary>
    public class EditModel : PageModel
    {
        private readonly DysFinContext _context;

        public EditModel(DysFinContext context)
        {
            _context = context;
        }

        [BindProperty]
        public JednostkaKontrolowana JednostkaKontrolowana { get; set; }

        /// <summary>
        /// Wyświetla formularz edycji danych jednostki kontrolowanej.
        /// </summary>
        /// <param name="id">Identyfikator jednostki kontrolowanej.</param>
        /// <returns>Strona z formularzem.</returns>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            JednostkaKontrolowana = await _context.JednostkaKontrolowana
                .Include(j => j.Proces)
                .Include(k => k.KomorkaMerytoryczna)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (JednostkaKontrolowana == null)
            {
                return NotFound();
            }

            ViewData["ProcesId"] = new SelectList(_context.SlownikProces, "Id", "Nazwa");
            ViewData["KomorkaMerytorycznaId"] = new SelectList(_context.Komorka, "Id", "Nazwa");

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", JednostkaKontrolowana.GetType().Name)
                .ForContext("RecordId", JednostkaKontrolowana.Id)
                .Warning("Otwarcie edycji jednostki kontrolowanej {Nazwa}.", JednostkaKontrolowana.Nazwa);

            return Page();
        }

        /// <summary>
        /// Przesyła model danych <see cref="Models.JednostkaKontrolowana"/> do zaktualizowania w aplikacji.
        /// </summary>
        /// <returns>Przekierowanie do listy jednostek kontrolowanych.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["ProcesId"] = new SelectList(_context.SlownikProces, "Id", "Nazwa");
                ViewData["KomorkaMerytorycznaId"] = new SelectList(_context.Komorka, "Id", "Nazwa");

                return Page();
            }

            _context.Attach(JednostkaKontrolowana).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JednostkaKontrolowanaExists(JednostkaKontrolowana.Id))
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
                .ForContext("Table", JednostkaKontrolowana.GetType().Name)
                .ForContext("RecordId", JednostkaKontrolowana.Id)
                .Warning("Zapisanie edycji jednostki kontrolowanej {Nazwa}.", JednostkaKontrolowana.Nazwa);

            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Sprawdza czy jednostka kontrolowana istnieje.
        /// </summary>
        /// <param name="id">Identyfikator jednostki kontrolowanej.</param>
        /// <returns>Prawda/Fałsz.</returns>
        private bool JednostkaKontrolowanaExists(int id)
        {
            return _context.JednostkaKontrolowana.Any(e => e.Id == id);
        }
    }
}
