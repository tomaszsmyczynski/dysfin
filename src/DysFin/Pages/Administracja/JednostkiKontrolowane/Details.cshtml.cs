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

namespace DysFin.Pages.Administracja.JednostkiKontrolowane
{
    /// <summary>
    /// Strona szczegółów jednostki kontrolowanej.
    /// </summary>
    public class DetailsModel : PageModel
    {
        private readonly DysFinContext _context;

        public DetailsModel(DysFinContext context)
        {
            _context = context;
        }

        public JednostkaKontrolowana JednostkaKontrolowana { get; set; }

        /// <summary>
        /// Wyświetla stronę z danymi jednostki.
        /// </summary>
        /// <param name="id">Identyfikator jednostki kontrolowanej.</param>
        /// <returns></returns>
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

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", JednostkaKontrolowana.GetType().Name)
                .ForContext("RecordId", JednostkaKontrolowana.Id)
                .Warning("Przegląd jednostki kontrolowanej {Nazwa}.", JednostkaKontrolowana.Nazwa);

            return Page();
        }
    }
}
