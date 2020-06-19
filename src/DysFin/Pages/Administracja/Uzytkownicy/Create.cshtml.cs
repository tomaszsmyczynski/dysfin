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

namespace DysFin.Pages.Administracja.Uzytkownicy
{
    /// <summary>
    /// Strona dodawania nowego użytkownika.
    /// </summary>
    public class CreateModel : PageModel
    {
        private readonly DysFinContext _context;

        public CreateModel(DysFinContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Wyświetla formularz dodawania nowego użytkownika.
        /// </summary>
        /// <returns>Strona z formularzem.</returns>
        public IActionResult OnGet()
        {
            ViewData["JednostkaId"] = new SelectList(_context.JednostkaKontrolowana, "Id", "Nazwa");
            ViewData["KomorkaId"] = new SelectList(_context.Komorka.OrderBy(k => k.Nazwa), "Id", "Nazwa");
            ViewData["PoziomUzytkownikaId"] = new SelectList(_context.SlownikPoziomUzytkownika, "Id", "Nazwa");

            return Page();
        }

        [BindProperty]
        public Uzytkownik Uzytkownik { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Przesyła model danych <see cref="Models.Uzytkownik"/> do aplikacji.
        /// </summary>
        /// <returns>Przekierowanie do listy użytkowników.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["JednostkaId"] = new SelectList(_context.JednostkaKontrolowana, "Id", "Nazwa");
                ViewData["KomorkaId"] = new SelectList(_context.Komorka.OrderBy(k => k.Nazwa), "Id", "Nazwa");
                ViewData["PoziomUzytkownikaId"] = new SelectList(_context.SlownikPoziomUzytkownika, "Id", "Nazwa");

                return Page();
            }

            _context.Uzytkownik.Add(Uzytkownik);
            await _context.SaveChangesAsync();

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", Uzytkownik.GetType().Name)
                .ForContext("RecordId", Uzytkownik.Id)
                .Warning("Dodanie użytkownika {Login}.", Uzytkownik.Login);

            return RedirectToPage("./Index");
        }
    }
}
