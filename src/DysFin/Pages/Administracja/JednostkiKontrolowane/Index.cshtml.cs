using AutoMapper;
using DysFin.Data;
using DysFin.Extensions;
using DysFin.Models;
using DysFin.Models.DysFinViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DysFin.Pages.Administracja.JednostkiKontrolowane
{
    /// <summary>
    /// Strona z listą jednostek kontrolowanych.
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
        /// Stronnnicowana lista <see cref="Models.JednostkaKontrolowana"/>.
        /// </summary>
        public PaginatedList<JednostkaKontrolowana> JednostkaKontrolowana { get;set; }

        /// <summary>
        /// Sortowanie po symbolu.
        /// </summary>
        public string SymbolSort { get; set; }

        /// <summary>
        /// Sortowanie po procesie.
        /// </summary>
        public string ProcesSort { get; set; }

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
        /// Wyświetla listę jednostek kontrolowanych.
        /// </summary>
        /// <param name="sortOrder">Kolejność sortowania.</param>
        /// <param name="currentFilter">Aktualny filtr.</param>
        /// <param name="searchString">Szukana fraza.</param>
        /// <param name="pageIndex">Indeks strony.</param>
        /// <returns>Strona z listą jednostek kontrolowanych.</returns>
        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            NazwaSort = string.IsNullOrEmpty(sortOrder) ? "Nazwa_desc" : "";
            SymbolSort = sortOrder == "Symbol" ? "Symbol_desc" : "Symbol";
            ProcesSort = sortOrder == "Proces" ? "Proces_desc" : "Proces";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<JednostkaKontrolowana> jednostkaKontrolowanaIQ = from j in _context.JednostkaKontrolowana
                                                                        select j;

            if (!string.IsNullOrEmpty(searchString))
            {
                jednostkaKontrolowanaIQ = jednostkaKontrolowanaIQ.Where(j => j.Nazwa.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "Nazwa_desc":
                    jednostkaKontrolowanaIQ = jednostkaKontrolowanaIQ.OrderByDescending(j => j.Nazwa);
                    break;
                case "Symbol":
                    jednostkaKontrolowanaIQ = jednostkaKontrolowanaIQ.OrderBy(j => j.Symbol);
                    break;
                case "Symbol_desc":
                    jednostkaKontrolowanaIQ = jednostkaKontrolowanaIQ.OrderByDescending(j => j.Symbol);
                    break;
                case "Proces":
                    jednostkaKontrolowanaIQ = jednostkaKontrolowanaIQ.OrderBy(j => j.Proces.Kod);
                    break;
                case "Proces_desc":
                    jednostkaKontrolowanaIQ = jednostkaKontrolowanaIQ.OrderByDescending(j => j.Proces.Kod);
                    break;
                default:
                    jednostkaKontrolowanaIQ = jednostkaKontrolowanaIQ.OrderBy(j => j.Nazwa);
                    break;
            }

            int pageSize = 15;
            JednostkaKontrolowana = await PaginatedList<JednostkaKontrolowana>.CreateAsync(
                jednostkaKontrolowanaIQ.AsNoTracking().Include(j => j.Proces).Include(k => k.KomorkaMerytoryczna), pageIndex ?? 1, pageSize);

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", nameof(JednostkaKontrolowana))
                .Warning("Przegląd listy jednostek kontrolowanych.");
        }

        public async Task<IActionResult> OnPostExportExcelAsync()
        {
            var list = await _context.JednostkaKontrolowana
                .Include(u => u.Proces)
                .Include(u => u.KomorkaMerytoryczna)
                .ToListAsync();

            var viewList = _mapper.Map<List<JednostkaKontrolowanaViewModel>>(list);

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Arkusz1");
                workSheet.Cells.LoadFromCollection(viewList, true, OfficeOpenXml.Table.TableStyles.Medium2);
                workSheet.Cells.AutoFitColumns();
                package.Save();
            }

            stream.Position = 0;

            string excelName = $"{nameof(JednostkiKontrolowane)}-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", nameof(JednostkaKontrolowana))
                .Warning("Pobranie listy jednostek kontrolowanych do pliku.");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}
