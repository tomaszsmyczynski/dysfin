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

namespace DysFin.Pages.Administracja.KomorkiKontrolowane
{
    /// <summary>
    /// Strona usuwania komórki kontrolowanej.
    /// </summary>
    public class DeleteModel : PageModel
    {
        private readonly DysFinContext _context;

        public DeleteModel(DysFinContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Komorka Komorka { get; set; }

        /// <summary>
        /// Wyświetla stronę potwierdzającą usunięcie komórki kontrolowanej.
        /// </summary>
        /// <param name="id">Identyfikator komórki kontrolowanej.</param>
        /// <returns>Strona usunięcia komórki.</returns>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Komorka = await _context.Komorka.FirstOrDefaultAsync(m => m.Id == id);

            if (Komorka == null)
            {
                return NotFound();
            }
            return Page();
        }

        /// <summary>
        /// Przesyła usunięcie obiektu <see cref="Models.Komorka"/> do aplikacji.
        /// </summary>
        /// <param name="id">Identyfikator komórki kontrolowanej.</param>
        /// <returns>Przekierowanie do listy komórek kontrolowanych.</returns>
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Komorka = await _context.Komorka.FindAsync(id);

            if (Komorka != null)
            {
                _context.Komorka.Remove(Komorka);
                await _context.SaveChangesAsync();

                Log
                    .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                    .ForContext("Table", Komorka.GetType().Name)
                    .ForContext("RecordId", Komorka.Id)
                    .Warning("Usunięcie komórki {Nazwa}.", Komorka.Nazwa);
            }

            return RedirectToPage("./Index");
        }
    }
}
