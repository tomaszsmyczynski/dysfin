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

namespace DysFin.Pages.Administracja.JednostkiKontrolujace
{
    /// <summary>
    /// Strona szczegółów jednostki kontrolującej.
    /// </summary>
    public class DetailsModel : PageModel
    {
        private readonly DysFinContext _context;

        public DetailsModel(DysFinContext context)
        {
            _context = context;
        }

        public JednostkaKontrolujaca JednostkaKontrolujaca { get; set; }

        /// <summary>
        /// Wyświetla stronę z danymi jednostki.
        /// </summary>
        /// <param name="id">Identyfikator jednostki kontrolującej.</param>
        /// <returns>Strona z danymi jednostki.</returns>
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
                .Warning("Przeglądanie jednostki kontrolującej {Nazwa}.", JednostkaKontrolujaca.Nazwa);

            return Page();
        }
    }
}
