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

namespace DysFin.Pages.Administracja.KomorkiKontrolowane
{
    /// <summary>
    /// Strona dodania nowej komórki kontrolowanej.
    /// </summary>
    public class CreateModel : PageModel
    {
        private readonly DysFinContext _context;

        public CreateModel(DysFinContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Wyświetla formularz dodawania nowej komórki.
        /// </summary>
        /// <returns>Strona z formularzem.</returns>
        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Komorka Komorka { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Przesyła model danych <see cref="Models.Komorka"/> do aplikacji.
        /// </summary>
        /// <returns>Przekierowanie do listy komórek kontrolowanych.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Komorka.Add(Komorka);
            await _context.SaveChangesAsync();

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", Komorka.GetType().Name)
                .ForContext("RecordId", Komorka.Id)
                .Warning("Dodanie komórki {Nazwa}.", Komorka.Nazwa);

            return RedirectToPage("./Index");
        }
    }
}
