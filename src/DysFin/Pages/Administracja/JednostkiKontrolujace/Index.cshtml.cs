using AutoMapper;
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

namespace DysFin.Pages.Administracja.JednostkiKontrolujace
{
    /// <summary>
    /// Strona z listą jednostek kontrolujących.
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
        /// Stronnnicowana lista <see cref="Models.JednostkaKontrolujaca"/>.
        /// </summary>
        public PaginatedList<JednostkaKontrolujaca> JednostkaKontrolujaca { get;set; }

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
        /// Wyświetla listę jednostek kontrolujących.
        /// </summary>
        /// <param name="sortOrder">Kolejność sortowania.</param>
        /// <param name="currentFilter">Aktualny filtr.</param>
        /// <param name="searchString">Szukana fraza.</param>
        /// <param name="pageIndex">Indeks strony.</param>
        /// <returns>Strona z listą jednostek kontrolujących.</returns>
        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            NazwaSort = string.IsNullOrEmpty(sortOrder) ? "Nazwa_desc" : "";
            if(searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<JednostkaKontrolujaca> jednostkaKontrolujacaIQ = from j in _context.JednostkaKontrolujaca
                                                                        select j;

            if(!string.IsNullOrEmpty(searchString))
            {
                jednostkaKontrolujacaIQ = jednostkaKontrolujacaIQ.Where(j => j.Nazwa.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "Nazwa_desc":
                    jednostkaKontrolujacaIQ = jednostkaKontrolujacaIQ.OrderByDescending(j => j.Nazwa);
                    break;
                default:
                    jednostkaKontrolujacaIQ = jednostkaKontrolujacaIQ.OrderBy(j => j.Nazwa);
                    break;
            }

            int pageSize = 10;
            JednostkaKontrolujaca = await PaginatedList<JednostkaKontrolujaca>.CreateAsync(
                jednostkaKontrolujacaIQ.AsNoTracking(), pageIndex ?? 1, pageSize);

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", nameof(JednostkaKontrolujaca))
                .Warning("Przegląd listy jednostek kontrolujących.");
        }

        public async Task<IActionResult> OnPostExportExcelAsync()
        {
            var list = await _context.JednostkaKontrolujaca
                .ToListAsync();

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Arkusz1");
                workSheet.Cells.LoadFromCollection(list, true, OfficeOpenXml.Table.TableStyles.Medium2);
                workSheet.Cells.AutoFitColumns();
                package.Save();
            }

            stream.Position = 0;

            string excelName = $"{nameof(JednostkiKontrolujace)}-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", nameof(JednostkaKontrolujaca))
                .Warning("Pobranie listy jednostek kontrolujących do pliku.");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}
