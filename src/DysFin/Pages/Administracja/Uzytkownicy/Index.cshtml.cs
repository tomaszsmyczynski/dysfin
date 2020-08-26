using AutoMapper;
using DysFin.Data;
using DysFin.Extensions;
using DysFin.Models;
using DysFin.Models.DysFinViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DysFin.Pages.Administracja.Uzytkownicy
{
    /// <summary>
    /// Strona z listą użytkowników.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly DysFinContext _context;
        private readonly IMapper _mapper;

        public IndexModel(DysFinContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Stronnnicowana lista <see cref="Models.Uzytkownik"/>.
        /// </summary>
        public PaginatedList<Uzytkownik> Uzytkownik { get; set; }

        [BindProperty]
        public SlownikPoziomUzytkownika PoziomUzytkownika { get; set; }

        [BindProperty]
        public Komorka Komorka { get; set; }

        /// <summary>
        /// Sortowanie po loginie użytkownika.
        /// </summary>
        public string LoginSort { get; set; }

        /// <summary>
        /// Sortowanie po poziomie użytkownika.
        /// </summary>
        public string PoziomSort { get; set; }

        /// <summary>
        /// Sortowanie po imieniu użytkownika.
        /// </summary>
        public string ImieSort { get; set; }

        /// <summary>
        /// Sortowanie po nazwisku użytkownika.
        /// </summary>
        public string NazwiskoSort { get; set; }

        /// <summary>
        /// Aktualny filtr tekstowy.
        /// </summary>
        public string CurrentFilter { get; set; }

        /// <summary>
        /// Aktualny filtr poziomu użytkownika.
        /// </summary>
        public int CurrentFilterPZ { get; set; }
        
        /// <summary>
        /// Aktualny filtr komórki organizacyjnej.
        /// </summary>
        public int CurrentFilterKomorka { get; set; }

        /// <summary>
        /// Aktualne sortowanie.
        /// </summary>
        public string CurrentSort { get; set; }

        /// <summary>
        /// Wyświetla listę użytkowników.
        /// </summary>
        /// <param name="sortOrder">Kolejność sortowania.</param>
        /// <param name="currentFilter">Aktualny filtr.</param>
        /// <param name="searchString">Szukana fraza.</param>
        /// <param name="pageIndex">Indeks strony.</param>
        /// <param name="poziomUzytkownika">Filtr poziomu użytkownika.</param>
        /// <param name="komorka">Filtr komórki organizacyjnej.</param>
        /// <returns>Strona z listą użytkowników.</returns>
        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex, int? poziomUzytkownika, int? komorka)
        {
            CurrentSort = sortOrder;
            var poziomy = _context.SlownikPoziomUzytkownika.AsNoTracking().ToList().Prepend(new SlownikPoziomUzytkownika()
            {
                Id = 0,
                Nazwa = "Wybierz"
            });
            ViewData["PoziomUzytkownikaId"] = new SelectList(poziomy, "Id", "Nazwa");

            var komorki = _context.Komorka.OrderBy(k => k.Nazwa).AsNoTracking().ToList().Prepend(new Komorka()
            {
                Id = 0,
                Nazwa = "Wybierz"
            });
            ViewData["KomorkaId"] = new SelectList(komorki, "Id", "Nazwa");

            NazwiskoSort = string.IsNullOrEmpty(sortOrder) ? "Nazwisko_desc" : "";
            LoginSort = sortOrder == "Login" ? "Login_desc" : "Login";
            PoziomSort = sortOrder == "Poziom" ? "Poziom_desc" : "Poziom";
            ImieSort = sortOrder == "Imie" ? "Imie_desc" : "Imie";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<Uzytkownik> uzytkownikIQ = from u in _context.Uzytkownik
                                                  select u;

            if (!string.IsNullOrEmpty(searchString))
            {
                uzytkownikIQ = uzytkownikIQ.Where(u => u.Login.Contains(searchString) || u.Nazwisko.Contains(searchString));
            }

            if (poziomUzytkownika.HasValue)
            {
                if (poziomUzytkownika != 0)
                {
                    uzytkownikIQ = uzytkownikIQ.Where(u => u.PoziomUzytkownikaId == poziomUzytkownika);
                    CurrentFilterPZ = poziomUzytkownika.Value;
                }
            }

            if (komorka.HasValue)
            {
                if (komorka != 0)
                {
                    uzytkownikIQ = uzytkownikIQ.Where(u => u.KomorkaId == komorka);
                    CurrentFilterKomorka = komorka.Value;
                }
            }

            uzytkownikIQ = sortOrder switch
            {
                "Nazwisko_desc" => uzytkownikIQ.OrderByDescending(u => u.Nazwisko),
                "Login" => uzytkownikIQ.OrderBy(u => u.Login),
                "Login_desc" => uzytkownikIQ.OrderByDescending(u => u.Login),
                "Poziom" => uzytkownikIQ.OrderBy(u => u.PoziomUzytkownika.Nazwa),
                "Poziom_desc" => uzytkownikIQ.OrderByDescending(u => u.PoziomUzytkownika.Nazwa),
                "Imie" => uzytkownikIQ.OrderBy(u => u.Imie),
                "Imie_desc" => uzytkownikIQ.OrderByDescending(u => u.Imie),
                _ => uzytkownikIQ.OrderBy(u => u.Nazwisko),
            };

            int pageSize = 10;
            Uzytkownik = await PaginatedList<Uzytkownik>.CreateAsync(
                uzytkownikIQ
                .Include(u => u.PoziomUzytkownika)
                //.Include(u => u.Jednostka)
                .Include(u => u.Komorka).AsNoTracking(), pageIndex ?? 1, pageSize);

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", nameof(Uzytkownik))
                .Warning("Przegląd listy użytkowników.");
        }

        public async Task<IActionResult> OnPostExportExcelAsync()
        {
            var list = await _context.Uzytkownik
                .Include(u => u.PoziomUzytkownika)
                //.Include(u => u.Jednostka)
                .Include(u => u.Komorka)
                .ToListAsync();

            var viewList = _mapper.Map<List<UzytkownikViewModel>>(list);

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Arkusz1");
                workSheet.Cells.LoadFromCollection(viewList, true, OfficeOpenXml.Table.TableStyles.Medium2);
                workSheet.Cells.AutoFitColumns();
                package.Save();
            }

            stream.Position = 0;

            string excelName = $"{nameof(Uzytkownicy)}-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", nameof(Uzytkownik))
                .Warning("Pobranie listy użytkowników do pliku.");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}