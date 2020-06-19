using DysFin.Data;
using DysFin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace DysFin.Pages.Kontrole
{
    /// <summary>
    /// Strona dodawania nowej kontroli zewnętrznej.
    /// </summary>
    public class CreateModel : PageModel
    {
        private readonly DysFinContext _context;

        public CreateModel(DysFinContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Wyświetla formularz dodawania nowej kontroli.
        /// </summary>
        /// <returns>Strona z formularzem.</returns>
        public IActionResult OnGet()
        {
            if (User.Claims.FirstOrDefault(u => u.Type.EndsWith("role")).Value == "2")
            {
                ViewData["JednostkaKontrolowanaId"] = new SelectList(PrepareJednostki(_context.JednostkaKontrolowana), "Id", "Nazwa");
                ViewData["KomorkaWiodacaId"] = new SelectList(_context.Komorka.OrderBy(k => k.Nazwa), "Id", "Nazwa").Prepend(new SelectListItem("Wybierz", "0"));
            }
            else
            {
                var komorka = _context.Komorka.AsEnumerable().Where(u => u.Id == int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("komorka")).Value));
                ViewData["JednostkaKontrolowanaId"] = new SelectList(_context.JednostkaKontrolowana.Where(j => j.Symbol == komorka.FirstOrDefault().Symbol), "Id", "Nazwa");
                ViewData["KomorkaWiodacaId"] = new SelectList(komorka, "Id", "Nazwa");
            }

            ViewData["JednostkaKontrolujacaId"] = new SelectList(_context.JednostkaKontrolujaca.OrderBy(j => j.Nazwa), "Id", "Nazwa").Prepend(new SelectListItem("Wybierz", "0"));
            ViewData["TypKontroliId"] = new SelectList(_context.SlownikTypKontroli.OrderBy(t => t.Nazwa), "Id", "Nazwa").Prepend(new SelectListItem("Wybierz", "0"));
            ViewData["StatusId"] = new SelectList(_context.SlownikStatusKontroli.Where(s => s.Id == 17 || s.Id == 40), "Id", "Nazwa").Prepend(new SelectListItem("Wybierz", "0"));
            ViewData["ProcesId"] = new SelectList(_context.SlownikProces, "Id", "Nazwa");

            return Page();
        }

        /// <summary>
        /// Zwraca listę jednostek kontrolowanych.
        /// </summary>
        /// <returns>Lista jednostek kontrolowanych.</returns>
        public JsonResult OnGetJednostki()
        {
            if (User.Claims.FirstOrDefault(u => u.Type.EndsWith("role")).Value == "2")
            {
                return new JsonResult(PrepareJednostki(_context.JednostkaKontrolowana));
            }
            else
            {
                var komorka = _context.Komorka.AsEnumerable().Where(u => u.Id == int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("komorka")).Value));
                return new JsonResult(_context.JednostkaKontrolowana.Where(j => j.Symbol == komorka.FirstOrDefault().Symbol));
            }
        }

        /// <summary>
        /// Zwraca listę komórek kontrolowanych.
        /// </summary>
        /// <returns>Lista komórek kontrolowanych.</returns>
        public JsonResult OnGetKomorki()
        {
            return new JsonResult(_context.Komorka.OrderBy(k => k.Nazwa));
        }

        /// <summary>
        /// Zwraca widok nowej pozycji dla komórek współpracujących.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Widok częściowy nowej pozycji.</returns>
        public IActionResult OnGetAddKomorka(int index)
        {
            return Partial("_AddKomorka", new KontrolaKomorki { Index = index });
        }

        [BindProperty]
        public Kontrola Kontrola { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Przesyła model danych <see cref="Models.Kontrola"/> do aplikacji.
        /// </summary>
        /// <returns>Przekierowanie do strony dodawania załączników kontroli.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (Kontrola.TypKontroliId == 0)
            {
                ModelState.AddModelError(string.Empty, "Wybierz typ kontroli.");
            }
            if (Kontrola.TypKontroliId == 2 & Kontrola.JednostkaKontrolowanaId == 0)
            {
                ModelState.AddModelError(string.Empty, "Wybierz jednostkę kontrolowaną.");
            }
            if (Kontrola.TypKontroliId == 1 & Kontrola.KomorkaWiodacaId == 0)
            {
                ModelState.AddModelError(string.Empty, "Wybierz komórkę wiodącą.");
            }
            if (Kontrola.JednostkaKontrolujacaId == 0)
            {
                ModelState.AddModelError(string.Empty, "Wybierz jednostkę kontrolującą.");
            }
            if (Kontrola.KomorkiUczestniczace.Count > 0)
            {
                if (Kontrola.KomorkiUczestniczace.Any(k => k.KomorkaId == 0))
                {
                    ModelState.AddModelError(string.Empty, "Wybierz komórkę uczestniczącą.");
                }
            }
            if (Kontrola.DataRozpoczecia > Kontrola.DataZakonczenia)
            {
                ModelState.AddModelError(string.Empty, "Data zakończenia musi być po dacie rozpoczęcia.");
            }
            if (Kontrola.OkresOd > Kontrola.OkresDo)
            {
                ModelState.AddModelError(string.Empty, "Data 'okres kontroli - koniec' musi być po dacie 'okres kontroli - start'.");
            }
            if(Kontrola.StatusId == 0)
            {
                ModelState.AddModelError(string.Empty, "Wybierz status kontroli.");
            }

            ModelStateEntry procesy = null;
            bool procesyWybrane = false;

            if (Kontrola.TypKontroliId == 1)
            {
                if (ModelState.TryGetValue("Kontrola.Procesy", out procesy))
                {
                    if (!string.IsNullOrEmpty(procesy.AttemptedValue))
                    {
                        procesyWybrane = true;
                    }
                    else
                    {
                        ModelState.AddModelError("Kontrola.Procesy", "Wybierz co najmniej jeden proces z listy.");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                if (User.Claims.FirstOrDefault(u => u.Type.EndsWith("role")).Value == "2")
                {
                    ViewData["JednostkaKontrolowanaId"] = new SelectList(PrepareJednostki(_context.JednostkaKontrolowana), "Id", "Nazwa");
                    ViewData["KomorkaWiodacaId"] = new SelectList(_context.Komorka.OrderBy(k => k.Nazwa), "Id", "Nazwa").Prepend(new SelectListItem("Wybierz", "0"));
                }
                else
                {
                    var komorka = _context.Komorka.AsEnumerable().Where(u => u.Id == int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("komorka")).Value));
                    ViewData["JednostkaKontrolowanaId"] = new SelectList(_context.JednostkaKontrolowana.Where(j => j.Symbol == komorka.FirstOrDefault().Symbol), "Id", "Nazwa");
                    ViewData["KomorkaWiodacaId"] = new SelectList(komorka, "Id", "Nazwa");
                }

                ViewData["JednostkaKontrolujacaId"] = new SelectList(_context.JednostkaKontrolujaca.OrderBy(j => j.Nazwa), "Id", "Nazwa").Prepend(new SelectListItem("Wybierz", "0"));
                ViewData["TypKontroliId"] = new SelectList(_context.SlownikTypKontroli.OrderBy(t => t.Nazwa), "Id", "Nazwa").Prepend(new SelectListItem("Wybierz", "0"));
                ViewData["StatusId"] = new SelectList(_context.SlownikStatusKontroli.Where(s => s.Id == 17 || s.Id == 40), "Id", "Nazwa").Prepend(new SelectListItem("Wybierz", "0"));
                ViewData["ProcesId"] = new SelectList(_context.SlownikProces, "Id", "Nazwa");

                return Page();
            }
            if (Kontrola.JednostkaKontrolowanaId.HasValue)
            {
                var jednostkaKontrolowanaSymbol = _context.JednostkaKontrolowana.Find(Kontrola.JednostkaKontrolowanaId).Symbol;
                Kontrola.KomorkaWiodacaId = _context.Komorka.FirstOrDefault(k => k.Symbol == jednostkaKontrolowanaSymbol).Id;
            }
            else
            {
                Kontrola.JednostkaKontrolowanaId = 1;
            }

            Kontrola.DataWpisu = DateTime.Now;

            var ostatniaKontrola = _context.Kontrola.AsEnumerable().LastOrDefault(k => k.DataWpisu.Year == DateTime.Now.Year);

            Kontrola.Lp = ostatniaKontrola?.Lp + 1 ?? 1;

            Kontrola.Rok = Kontrola.DataWpisu.Year;

            _context.Kontrola.Add(Kontrola);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                return BadRequest(ex);
            }

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", Kontrola.GetType().Name)
                .ForContext("RecordId", Kontrola.Id)
                .Warning("Dodanie kontroli {Numer}.", Kontrola.Numer);

            if (procesyWybrane)
            {
                if (procesy.RawValue.ToString().Length > 1)
                {
                    foreach (var proces in (string[])procesy.RawValue)
                    {
                        Kontrola.Procesy.Add(new KontrolaProcesy()
                        {
                            KontrolaId = Kontrola.Id,
                            ProcesId = int.Parse(proces)
                        });
                    }
                }
                else
                {
                    Kontrola.Procesy.Add(new KontrolaProcesy()
                    {
                        KontrolaId = Kontrola.Id,
                        ProcesId = int.Parse(procesy.RawValue.ToString())
                    });
                }
            }
            else
            {
                var procesId = _context.Komorka.FirstOrDefault(k => k.Symbol == Kontrola.JednostkaKontrolowana.Symbol)?.ProcesId ?? Kontrola.JednostkaKontrolowana.ProcesId;

                Kontrola.Procesy.Add(new KontrolaProcesy()
                {
                    KontrolaId = Kontrola.Id,
                    ProcesId = procesId
                });
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                return BadRequest(ex);
            }

            if (Kontrola.StatusId == 17)
            {
                return RedirectToPage("./Details", new { id = Kontrola.Id });
            }
            else if (Kontrola.StatusId == 40)
            {
                return RedirectToPage("./Zalaczniki", new { id = Kontrola.Id, edit = false });
            }
            else
            {
                return RedirectToPage("./Nieprawidlowosci", new { id = Kontrola.Id });
            }
        }

        /// <summary>
        /// <see cref="OnGetJednostki"/>
        /// </summary>
        /// <param name="jednostki">Zestaw jednostek z bazy danych.</param>
        /// <returns>Lista jednostek kontrolowanych.</returns>
        private IEnumerable<JednostkaKontrolowana> PrepareJednostki(Microsoft.EntityFrameworkCore.DbSet<JednostkaKontrolowana> jednostki)
        {
            var jednostkiKontrolowane = jednostki.ToList();
            jednostkiKontrolowane.RemoveAt(0);

            return jednostkiKontrolowane.OrderBy(j => j.Nazwa).Prepend(new JednostkaKontrolowana()
            {
                Id = 0,
                Nazwa = "Wybierz"
            });
        }
    }
}
