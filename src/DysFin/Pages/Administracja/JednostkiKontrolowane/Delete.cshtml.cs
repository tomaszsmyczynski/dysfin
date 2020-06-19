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
    /// Strona usuwania jednostki kontrolowanej.
    /// </summary>
    public class DeleteModel : PageModel
    {
        private readonly DysFinContext _context;

        public DeleteModel(DysFinContext context)
        {
            _context = context;
        }

        [BindProperty]
        public JednostkaKontrolowana JednostkaKontrolowana { get; set; }

        /// <summary>
        /// Wyświetla stronę potwierdzającą usunięcie jednostki kontrolowanej.
        /// </summary>
        /// <param name="id">Identyfikator jednostki kontrolowanej.</param>
        /// <returns>Strona usunięcia jednostki.</returns>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            JednostkaKontrolowana = await _context.JednostkaKontrolowana
                .Include(j => j.Proces).FirstOrDefaultAsync(m => m.Id == id);

            if (JednostkaKontrolowana == null)
            {
                return NotFound();
            }
            return Page();
        }

        /// <summary>
        /// Przesyła usunięcie obiektu <see cref="Models.JednostkaKontrolowana"/> do aplikacji.
        /// </summary>
        /// <param name="id">Identyfikator jednostki kontrolowanej.</param>
        /// <returns>Przekierowanie do listy jednostek kontrolowanych.</returns>
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            JednostkaKontrolowana = await _context.JednostkaKontrolowana.FindAsync(id);

            if (JednostkaKontrolowana != null)
            {
                _context.JednostkaKontrolowana.Remove(JednostkaKontrolowana);
                await _context.SaveChangesAsync();

                Log
                    .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                    .ForContext("Table", JednostkaKontrolowana.GetType().Name)
                    .ForContext("RecordId", JednostkaKontrolowana.Id)
                    .Warning("Usunięcie jednostki kontrolowanej {Nazwa}.", JednostkaKontrolowana.Nazwa);
            }

            return RedirectToPage("./Index");
        }
    }
}
