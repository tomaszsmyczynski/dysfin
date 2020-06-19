using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DysFin.Data;
using DysFin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DysFin.Web.Pages.Shared.Components.KontrolaDetails
{
    public class KontrolaDetails : ViewComponent
    {
        private readonly DysFinContext _context;

        public KontrolaDetails(DysFinContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Wyœwietla stronê ze szczegó³ami kontroli.
        /// </summary>
        /// <param name="id">Identyfikator kontroli.</param>
        /// <returns>Strona z danymi kontroli.</returns>
        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            Kontrola kontrola = await _context.Kontrola
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

            return View(kontrola);
        }
    }
}
