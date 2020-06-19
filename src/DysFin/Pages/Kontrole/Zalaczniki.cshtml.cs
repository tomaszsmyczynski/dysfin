using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DysFin.Data;
using DysFin.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DysFin.Pages.Kontrole
{
    /// <summary>
    /// Strona dodawania załączników do kontroli.
    /// </summary>
    public class ZalacznikiModel : PageModel
    {
        private readonly DysFinContext _context;
        /// <summary>
        /// Limit wielkości pliku załącznika.
        /// </summary>
        private readonly long _fileSizeLimit;

        public ZalacznikiModel(DysFinContext context,
            IConfiguration config)
        {
            _context = context;
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
        }

        /// <summary>
        /// Wyświetla formularz dodawania załącznika.
        /// </summary>
        /// <param name="id">Identyfikator kontroli.</param>
        /// <param name="edit"><see cref="Edit"/></param>
        /// <returns></returns>
        public IActionResult OnGet(int id, bool edit)
        {
            if (Plik != null)
            {
                Plik = null;
                ModelState.Clear();
            }

            ViewData["KontrolaId"] = id;
            return Page();
        }

        /// <summary>
        /// Pobiera załącznik.
        /// </summary>
        /// <param name="id">Identyfikator załącznika.</param>
        /// <returns>Plik załącznika.</returns>
        public IActionResult OnGetFile(int id)
        {
            var zalacznik = _context.Zalacznik.Find(id);

            zalacznik.Kontrola = _context.Kontrola.Find(zalacznik.KontrolaId);

            return File(
                zalacznik.Zawartosc, 
                zalacznik.TypZawartosci, 
                string.Concat(
                    zalacznik.Kontrola.Lp, 
                    '%', zalacznik.Kontrola.Rok, 
                    '_', zalacznik.Kontrola.JednostkaKontrolowanaId, 
                    '_', zalacznik.Kontrola.Zalaczniki.ToList().IndexOf(zalacznik) + 1, 
                    '_', zalacznik.Nazwa));
        }

        /// <summary>
        /// Usuwa załącznik.
        /// </summary>
        /// <param name="id">Identyfikator załącznika.</param>
        /// <param name="kontrolaId">Identyfikator powiązanej kontroli.</param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetDelete(int? id, int kontrolaId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zalacznik = await _context.Zalacznik.FindAsync(id);

            if (zalacznik != null)
            {
                _context.Zalacznik.Remove(zalacznik);
                await _context.SaveChangesAsync();
            }

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", zalacznik.GetType().Name)
                .ForContext("RecordId", zalacznik.Id)
                .ForContext("MainRecordId", kontrolaId)
                .Warning("Usunięcie załącznika {Nazwa}.", zalacznik.Nazwa);

            return RedirectToPage("./Edit", new { id = kontrolaId });
        }

        [BindProperty]
        public Plik Plik { get; set; }

        /// <summary>
        /// Informacja o źródle odesłania.
        /// </summary>
        [BindProperty]
        public bool Edit { get; set; }

        /// <summary>
        /// Wynik walidacji.
        /// </summary>
        public string Result { get; private set; }

        /// <summary>
        /// Przesyła model danych <see cref="Models.Kontrola"/> do aplikacji.
        /// </summary>
        /// <param name="edit"><see cref="Edit"/></param>
        /// <returns>Odświeżenie strony.</returns>
        public async Task<IActionResult> OnPostUploadAsync(bool edit)
        {
            // Perform an initial check to catch FileUpload class
            // attribute violations.
            if (!ModelState.IsValid)
            {
                Result = "Proszę poprawić formularz.";

                return Page();
            }

            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    await Plik.FormFile.CopyToAsync(memoryStream);
                }
                catch(Exception ex)
                {
                    Log.ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                        .ForContext("Table", nameof(Zalacznik))
                        .Error(ex, "Wystąpił błąd wysyłania pliku.");
                }

                // Upload the file if less than file size limit
                if (memoryStream.Length < _fileSizeLimit)
                {

                    var zalacznik = new Zalacznik
                    {
                        Nazwa = Plik.FormFile.FileName,
                        KontrolaId = Plik.KontrolaId,
                        Zawartosc = memoryStream.ToArray(),
                        Opis = Plik.Opis,
                        Rozmiar = Plik.FormFile.Length,
                        TypZawartosci = Plik.FormFile.ContentType,
                        Data = Plik.Data
                    };

                    _context.Zalacznik.Add(zalacznik);
                    await _context.SaveChangesAsync();

                    Log
                        .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                        .ForContext("Table", zalacznik.GetType().Name)
                        .ForContext("RecordId", zalacznik.Id)
                        .ForContext("MainRecordId", zalacznik.KontrolaId)
                        .Warning("Dodanie załącznika {Nazwa}.", zalacznik.Nazwa);
                }
                else
                {
                    ModelState.AddModelError("Plik.FormFile", "Plik jest większy niż dozwolone 8MB.");

                    ViewData["KontrolaId"] = Plik.KontrolaId;
                    return Page();
                }
            }

            if (Edit)
            {
                return OnGet(Plik.KontrolaId, edit);
            }
            else
            {
                return RedirectToPage("./Details", new { id = Plik.KontrolaId });
            }
        }
    }

    /// <summary>
    /// Model danych pliku załącznika.
    /// </summary>
    public class Plik
    {
        /// <summary>
        /// Identyfikator kontroli.
        /// </summary>
        public int KontrolaId { get; set; }

        /// <summary>
        /// Załącznik.
        /// </summary>
        [Required]
        [Display(Name = "Załącznik")]
        public IFormFile FormFile { get; set; }

        /// <summary>
        /// Opis.
        /// </summary>
        [Display(Name = "Opis")]
        public string Opis { get; set; }

        /// <summary>
        /// Data.
        /// </summary>
        [Required]
        [Display(Name = "Data")]
        [DataType(DataType.Date)]
        public DateTime Data { get; set; }

        /// <summary>
        /// Typ załącznika.
        /// </summary>
        [Required]
        [Display(Name = "Typ załącznika")]
        public int TypZalacznikaId { get; set; }
    }
}
