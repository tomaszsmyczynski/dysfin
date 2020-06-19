using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DysFin.Data;
using DysFin.Models;
using Serilog;

namespace DysFin.Pages.Administracja.JednostkiKontrolowane
{
    /// <summary>
    /// Strona dodania nowej jednostki kontrolowanej.
    /// </summary>
    public class CreateModel : PageModel
    {
        private readonly DysFinContext _context;

        public CreateModel(DysFinContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Wyświetla formularz dodawania nowej jednostki.
        /// </summary>
        /// <returns>Strona z formularzem.</returns>
        public IActionResult OnGet()
        {
            ViewData["ProcesId"] = new SelectList(_context.SlownikProces, "Id", "Nazwa");
            ViewData["KomorkaMerytorycznaId"] = new SelectList(_context.Komorka, "Id", "Nazwa");

            return Page();
        }

        [BindProperty]
        public JednostkaKontrolowana JednostkaKontrolowana { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Przesyła model danych <see cref="Models.JednostkaKontrolowana"/> do aplikacji.
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

            _context.JednostkaKontrolowana.Add(JednostkaKontrolowana);
            await _context.SaveChangesAsync();

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", JednostkaKontrolowana.GetType().Name)
                .ForContext("RecordId", JednostkaKontrolowana.Id)
                .Warning("Dodanie jednostki kontrolowanej {Nazwa}.", JednostkaKontrolowana.Nazwa);

            return RedirectToPage("./Index");
        }
    }
}
