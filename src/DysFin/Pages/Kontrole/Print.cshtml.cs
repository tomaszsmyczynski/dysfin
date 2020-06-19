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

namespace DysFin.Web.Pages.Kontrole
{
    /// <summary>
    /// Strona wydruku kontroli zewnętrznej.
    /// </summary>
    public class PrintModel : PageModel
    {
        private readonly DysFinContext _context;

        public PrintModel(DysFinContext context)
        {
            _context = context;
        }

        public Kontrola Kontrola { get; set; }

        /// <summary>
        /// Wyświetla nowe okno do druku.
        /// </summary>
        /// <param name="id">Identyfikator kontroli.</param>
        /// <returns>Strona z danymi kontroli.</returns>
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

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", Kontrola.GetType().Name)
                .ForContext("RecordId", Kontrola.Id)
                .Warning("Otwarcie do druku kontroli {Numer}.", Kontrola.Numer);

            return Page();
        }
    }
}
