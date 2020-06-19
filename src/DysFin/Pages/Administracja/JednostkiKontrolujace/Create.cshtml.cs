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

namespace DysFin.Pages.Administracja.JednostkiKontrolujace
{
    /// <summary>
    /// Strona dodania nowej jednostki kontrolującej.
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
            return Page();
        }

        [BindProperty]
        public JednostkaKontrolujaca JednostkaKontrolujaca { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Przesyła model danych <see cref="Models.JednostkaKontrolujaca"/> do aplikacji.
        /// </summary>
        /// <returns>Przekierowanie do listy jednostek kontrolujących.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.JednostkaKontrolujaca.Add(JednostkaKontrolujaca);
            await _context.SaveChangesAsync();

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", JednostkaKontrolujaca.GetType().Name)
                .ForContext("RecordId", JednostkaKontrolujaca.Id)
                .Warning("Dodanie jednostki kontrolującej {Nazwa}.", JednostkaKontrolujaca.Nazwa);

            return RedirectToPage("./Index");
        }
    }
}
