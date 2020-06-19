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
    /// Strona usuwania jednostki kontrolującej.
    /// </summary>
    public class DeleteModel : PageModel
    {
        private readonly DysFinContext _context;

        public DeleteModel(DysFinContext context)
        {
            _context = context;
        }

        [BindProperty]
        public JednostkaKontrolujaca JednostkaKontrolujaca { get; set; }

        /// <summary>
        /// Wyświetla stronę potwierdzającą usunięcie jednostki kontrolującej.
        /// </summary>
        /// <param name="id">Identyfikator jednostki kontrolującej.</param>
        /// <returns>Strona usunięcia jednostki.</returns>
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
            return Page();
        }

        /// <summary>
        /// Przesyła usunięcie obiektu <see cref="Models.JednostkaKontrolujaca"/> do aplikacji.
        /// </summary>
        /// <param name="id">Identyfikator jednostki kontrolującej.</param>
        /// <returns>Przekierowanie do listy jednostek kontrolujących.</returns>
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            JednostkaKontrolujaca = await _context.JednostkaKontrolujaca.FindAsync(id);

            if (JednostkaKontrolujaca != null)
            {
                _context.JednostkaKontrolujaca.Remove(JednostkaKontrolujaca);
                await _context.SaveChangesAsync();

                Log
                    .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                    .ForContext("Table", JednostkaKontrolujaca.GetType().Name)
                    .ForContext("RecordId", JednostkaKontrolujaca.Id)
                    .Warning("Usunięcie jednostki kontrolującej {Nazwa}.", JednostkaKontrolujaca.Nazwa);

            }

            return RedirectToPage("./Index");
        }
    }
}
