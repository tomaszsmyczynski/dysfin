using DysFin.Data;
using DysFin.Extensions;
using DysFin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DysFin.Pages.Administracja.KomorkiKontrolowane
{
    /// <summary>
    /// Strona z listą komórek kontrolowanych.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly DysFinContext _context;

        public IndexModel(DysFinContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Stronnnicowana lista <see cref="Models.Komorka"/>.
        /// </summary>
        public PaginatedList<Komorka> Komorka { get;set; }

        /// <summary>
        /// Sortowanie po symbolu.
        /// </summary>
        public string SymbolSort { get; set; }

        /// <summary>
        /// Sortowanie po nazwie.
        /// </summary>
        public string NazwaSort { get; set; }

        /// <summary>
        /// Aktualny filtr tekstowy.
        /// </summary>
        public string CurrentFilter { get; set; }

        /// <summary>
        /// Aktualne sortowanie.
        /// </summary>
        public string CurrentSort { get; set; }

        /// <summary>
        /// Wyświetla listę komórek kontrolowanych.
        /// </summary>
        /// <param name="sortOrder">Kolejność sortowania.</param>
        /// <param name="currentFilter">Aktualny filtr.</param>
        /// <param name="searchString">Szukana fraza.</param>
        /// <param name="pageIndex">Indeks strony.</param>
        /// <returns>Strona z listą komórek kontrolowanych.</returns>
        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            NazwaSort = string.IsNullOrEmpty(sortOrder) ? "Nazwa_desc" : "";
            SymbolSort = sortOrder == "Symbol" ? "Symbol_desc" : "Symbol";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<Komorka> komorkaIQ = from k in _context.Komorka
                                                          select k;
            if (!string.IsNullOrEmpty(searchString))
            {
                komorkaIQ = komorkaIQ.Where(k => k.Nazwa.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "Nazwa_desc":
                    komorkaIQ = komorkaIQ.OrderByDescending(k => k.Nazwa);
                    break;
                case "Symbol":
                    komorkaIQ = komorkaIQ.OrderBy(k => k.Symbol);
                    break;
                case "Symbol_desc":
                    komorkaIQ = komorkaIQ.OrderByDescending(k => k.Symbol);
                    break;
                default:
                    komorkaIQ = komorkaIQ.OrderBy(k => k.Nazwa);
                    break;
            }

            int pageSize = 10;
            Komorka = await PaginatedList<Komorka>.CreateAsync(
                komorkaIQ.AsNoTracking(), pageIndex ?? 1, pageSize);

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", nameof(Komorka))
                .Warning("Przegląd listy komórek.");
        }

        public async Task<IActionResult> OnPostExportExcelAsync()
        {
            var list = await _context.Komorka
                .ToListAsync();

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Arkusz1");
                workSheet.Cells.LoadFromCollection(list, true, OfficeOpenXml.Table.TableStyles.Medium2);
                workSheet.Cells.AutoFitColumns();
                workSheet.DeleteColumn(2);
                package.Save();
            }

            stream.Position = 0;

            string excelName = $"{nameof(KomorkiKontrolowane)}-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", nameof(Komorka))
                .Warning("Pobranie listy komórek kontrolowanych do pliku.");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}
