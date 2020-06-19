using AutoMapper;
using DysFin.Data;
using DysFin.Extensions;
using DysFin.Models;
using DysFin.Models.DysFinViewModels;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using OfficeOpenXml;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DysFin.Pages.Kontrole
{
    /// <summary>
    /// Strona z listą kontroli zewnętrznych.
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
        /// 
        /// Stronnnicowana lista <see cref="Models.Kontrola"/>.
        /// </summary>
        public PaginatedList<Kontrola> Kontrola { get;set; }

        [BindProperty]
        public SlownikStatusKontroli Status { get; set; }

        [BindProperty]
        public Komorka Komorka { get; set; }

        [BindProperty]
        public JednostkaKontrolowana JednostkaKontrolowana { get; set; }

        [BindProperty]
        public JednostkaKontrolujaca JednostkaKontrolujaca { get; set; }

        /// <summary>
        /// Sortowanie po numerze.
        /// </summary>
        public string NumerSort { get; set; }

        /// <summary>
        /// Sortowanie po jednostce kontrolującej.
        /// </summary>
        public string JednostkaKontrolujacaSort { get; set; }

        /// <summary>
        /// Sortowanie po jednostce kontrolowanej.
        /// </summary>
        public string JednostkaKontrolowanaSort { get; set; }

        /// <summary>
        /// Sortowanie po komórce wiodącej.
        /// </summary>
        public string KomorkaWiodacaSort { get; set; }

        /// <summary>
        /// Sortowanie po dacie rozpoczęcia.
        /// </summary>
        public string DataRozpoczeciaSort { get; set; }

        /// <summary>
        /// Sortowanie po dacie zakończenia.
        /// </summary>
        public string DataZakonczeniaSort { get; set; }

        /// <summary>
        /// Sortowanie po statusie.
        /// </summary>
        public string StatusSort { get; set; }

        /// <summary>
        /// Aktualny filtr tekstowy.
        /// </summary>
        public string CurrentFilter { get; set; }

        /// <summary>
        /// Aktualny filtr jednostki kontrolującej.
        /// </summary>
        public int CurrentFilterKontrolujaca { get; set; }

        /// <summary>
        /// Aktualny filtr jednostki kontrolowanej.
        /// </summary>
        public int CurrentFilterKontrolowana { get; set; }

        /// <summary>
        /// Aktualny filtr statusu.
        /// </summary>
        public int CurrentFilterStatus { get; set; }

        /// <summary>
        /// Aktualny filtr komórki.
        /// </summary>
        public int CurrentFilterKomorka { get; set; }

        /// <summary>
        /// Aktualne sortowanie.
        /// </summary>
        public string CurrentSort { get; set; }

        //public string Result { get; private set; }

        /// <summary>
        /// Wyświetla listę kontroli.
        /// </summary>
        /// <param name="sortOrder">Kolejność sortowania.</param>
        /// <param name="currentFilter">Aktualny filtr.</param>
        /// <param name="searchString">Szukana fraza.</param>
        /// <param name="pageIndex">Indeks strony.</param>
        /// <param name="jednostkaKontrolujaca">Aktualny filtr jednostki kontrolującej.</param>
        /// <param name="jednostkaKontrolowana">Aktualny filtr jednostki kontrolowanej.</param>
        /// <param name="status">Aktualny filtr statusu.</param>
        /// <param name="komorka">Aktualny filtr komórki.</param>
        /// <returns>Strona z listą kontroli.</returns>
        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex, int? jednostkaKontrolujaca, int? jednostkaKontrolowana, int? status, int? komorka, string closed)
        {
            ViewData["Closed"] = closed;

            IQueryable<Kontrola> kontrolaIQ = from k in _context.Kontrola
                                              select k;

            IEnumerable<JednostkaKontrolowana> jednostkiKontrolowane;

            var komorkaId = int.Parse(User.Claims.FirstOrDefault(u => u.Type == "komorka").Value);
            var role = User.Claims.FirstOrDefault(u => u.Type.EndsWith("role")).Value;
            if (role == "2" || role == "5")
            {
                jednostkiKontrolowane = _context.JednostkaKontrolowana.OrderBy(k => k.Nazwa).AsNoTracking().ToList().Where(j => j.Nazwa != "Urząd Miasta").Prepend(new JednostkaKontrolowana()
                {
                    Id = 0,
                    Nazwa = "Wybierz"
                });
            }
            else
            {
                kontrolaIQ = kontrolaIQ.Where(k => k.KomorkaWiodacaId == komorkaId);

                jednostkiKontrolowane = _context.JednostkaKontrolowana.OrderBy(k => k.Nazwa).AsNoTracking().ToList().Where(j => j.Nazwa != "Urząd Miasta" && j.Symbol == _context.Komorka.Find(komorkaId).Symbol).Prepend(new JednostkaKontrolowana()
                {
                    Id = 0,
                    Nazwa = "Wybierz"
                });
            }
            CurrentSort = sortOrder;
            var statusy = _context.SlownikStatusKontroli.AsNoTracking().ToList().Prepend(new SlownikStatusKontroli()
            {
                Id = 0,
                Nazwa = "Wybierz"
            });
            ViewData["StatusId"] = new SelectList(statusy, "Id", "Nazwa");

            ViewData["JednostkaKontrolowanaId"] = new SelectList(jednostkiKontrolowane, "Id", "Nazwa");

            var jednostkiKontrolujace = _context.JednostkaKontrolujaca.OrderBy(k => k.Nazwa).AsNoTracking().ToList().Prepend(new JednostkaKontrolujaca()
            {
                Id = 0,
                Nazwa = "Wybierz"
            });

            ViewData["JednostkaKontrolujacaId"] = new SelectList(jednostkiKontrolujace, "Id", "Nazwa");

            var komorki = _context.Komorka.OrderBy(k => k.Nazwa).AsNoTracking().ToList().Prepend(new Komorka()
            {
                Id = 0,
                Nazwa = "Wybierz"
            });

            ViewData["KomorkaId"] = new SelectList(komorki, "Id", "Nazwa");

            NumerSort = sortOrder == "Numer" ? "Numer_desc" : "Numer"; 
            JednostkaKontrolujacaSort = sortOrder == "JednostkaKontrolujaca" ? "JednostkaKontrolujaca_desc" : "JednostkaKontrolujaca";
            JednostkaKontrolowanaSort = sortOrder == "JednostkaKontrolowana" ? "JednostkaKontrolowana_desc" : "JednostkaKontrolowana";
            KomorkaWiodacaSort = sortOrder == "KomorkaWiodaca" ? "KomorkaWiodaca_desc" : "KomorkaWiodaca";
            DataRozpoczeciaSort = sortOrder == "DataRozpoczecia" ? "DataRozpoczecia_desc" : "DataRozpoczecia";
            DataZakonczeniaSort = sortOrder == "DataZakonczenia" ? "DataZakonczenia_desc" : "DataZakonczenia";
            StatusSort = sortOrder == "Status" ? "Status_desc" : "Status";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                if(searchString.StartsWith('/'))
                {
                    kontrolaIQ = kontrolaIQ.Where(k => k.Rok.ToString() == searchString.TrimStart('/'));
                }
                else
                {
                    kontrolaIQ = kontrolaIQ.Where(k => k.Lp.ToString().Contains(searchString));
                }
                
            }

            //if (Request.Query.ContainsKey("DataRozpoczeciaOd"))
            //{
            //    Request.Query.TryGetValue("DataRozpoczeciaOd", out StringValues dataRozpoczeciaOd);
            //    if (!string.IsNullOrEmpty(dataRozpoczeciaOd))
            //        kontrolaIQ = kontrolaIQ.Where(k => k.DataRozpoczecia >= DateTime.Parse(dataRozpoczeciaOd.FirstOrDefault()));
            //}

            //if (Request.Query.ContainsKey("DataRozpoczeciaDo"))
            //{
            //    Request.Query.TryGetValue("DataRozpoczeciaDo", out StringValues dataRozpoczeciaDo);
            //    if (!string.IsNullOrEmpty(dataRozpoczeciaDo))
            //        kontrolaIQ = kontrolaIQ.Where(k => k.DataRozpoczecia <= DateTime.Parse(dataRozpoczeciaDo.FirstOrDefault()));
            //}

            //if (Request.Query.ContainsKey("DataZakonczeniaOd"))
            //{
            //    Request.Query.TryGetValue("DataZakonczeniaOd", out StringValues dataZakonczeniaOd);
            //    if (!string.IsNullOrEmpty(dataZakonczeniaOd))
            //        kontrolaIQ = kontrolaIQ.Where(k => k.DataZakonczenia >= DateTime.Parse(dataZakonczeniaOd.FirstOrDefault()));
            //}

            //if (Request.Query.ContainsKey("DataZakonczeniaDo"))
            //{
            //    Request.Query.TryGetValue("DataZakonczeniaDo", out StringValues dataZakonczeniaDo);
            //    if (!string.IsNullOrEmpty(dataZakonczeniaDo))
            //        kontrolaIQ = kontrolaIQ.Where(k => k.DataZakonczenia <= DateTime.Parse(dataZakonczeniaDo.FirstOrDefault()));
            //}

            if (status.HasValue)
            {
                if (status != 0)
                {
                    kontrolaIQ = kontrolaIQ.Where(k => k.StatusId == status);
                    CurrentFilterStatus = status.Value;
                }
            }

            if (jednostkaKontrolowana.HasValue)
            {
                if (jednostkaKontrolowana != 0)
                {
                    kontrolaIQ = kontrolaIQ.Where(k => k.JednostkaKontrolowanaId == jednostkaKontrolowana);
                    CurrentFilterKontrolowana = jednostkaKontrolowana.Value;
                }
            }

            if (jednostkaKontrolujaca.HasValue)
            {
                if (jednostkaKontrolujaca != 0)
                {
                    kontrolaIQ = kontrolaIQ.Where(k => k.JednostkaKontrolujacaId == jednostkaKontrolujaca);
                    CurrentFilterKontrolujaca = jednostkaKontrolujaca.Value;
                }
            }

            if (komorka.HasValue)
            {
                if (komorka != 0)
                {
                    kontrolaIQ = kontrolaIQ.Where(k => k.KomorkaWiodacaId == komorka);
                    CurrentFilterKomorka = komorka.Value;
                }
            }

            kontrolaIQ = sortOrder switch
            {
                "Numer" => kontrolaIQ.OrderBy(k => k.Rok).ThenBy(k => k.Lp),
                "Numer_desc" => kontrolaIQ.OrderByDescending(k => k.Rok).ThenByDescending(k => k.Lp),
                "JednostkaKontrolujaca" => kontrolaIQ.OrderBy(k => k.JednostkaKontrolujaca.Nazwa),
                "JednostkaKontrolujaca_desc" => kontrolaIQ.OrderByDescending(k => k.JednostkaKontrolujaca.Nazwa),
                "JednostkaKontrolowana" => kontrolaIQ.OrderBy(k => k.JednostkaKontrolowana.Nazwa),
                "JednostkaKontrolowana_desc" => kontrolaIQ.OrderByDescending(k => k.JednostkaKontrolowana.Nazwa),
                "KomorkaWiodaca" => kontrolaIQ.OrderBy(k => k.KomorkaWiodaca.Nazwa),
                "KomorkaWiodaca_desc" => kontrolaIQ.OrderByDescending(k => k.KomorkaWiodaca.Nazwa),
                "DataRozpoczecia" => kontrolaIQ.OrderBy(k => k.DataRozpoczecia),
                "DataRozpoczecia_desc" => kontrolaIQ.OrderByDescending(k => k.DataRozpoczecia),
                "DataZakonczenia" => kontrolaIQ.OrderBy(k => k.DataZakonczenia),
                "DataZakonczenia_desc" => kontrolaIQ.OrderByDescending(k => k.DataZakonczenia),
                "Status" => kontrolaIQ.OrderBy(k => k.Status.Nazwa),
                "Status_desc" => kontrolaIQ.OrderByDescending(k => k.Status.Nazwa),
                _ => kontrolaIQ.OrderBy(k => k.Id),
            };
            int pageSize = 15;
            Kontrola = await PaginatedList<Kontrola>.CreateAsync(
                kontrolaIQ
                .Include(k => k.JednostkaKontrolowana)
                .Include(k => k.JednostkaKontrolujaca)
                .Include(k => k.KomorkaWiodaca)
                .Include(k => k.Status)
                .Include(k => k.TypKontroli)
                .AsNoTracking(), pageIndex ?? 1, pageSize);

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", nameof(Kontrola))
                .Warning("Przegląd listy kontroli.");
        }

        /// <summary>
        /// Przechodzi do szczegółów podanej kontroli zewnętrzenej.
        /// </summary>
        /// <param name="numer">Numer kontroli.</param>
        /// <returns>Strona szczegółów kontroli.</returns>
        public ContentResult OnPost(string numer)
        {
            var kontrola = _context.Kontrola.FirstOrDefault(k => k.Lp.ToString() + '/' + k.Rok.ToString() == numer);
            if (kontrola != null)
            {
                return new ContentResult() { Content = Url.PageLink("Details", null, new { id = kontrola.Id }) };
            }
            else
            {
                return new ContentResult() { Content = Request.GetDisplayUrl() };
            }
        }

        public async Task<IActionResult> OnPostExportExcelAsync()
        {
            IQueryable<Kontrola> kontrolaIQ = from k in _context.Kontrola
                                              select k;

            //var komorkaId = int.Parse(User.Claims.FirstOrDefault(u => u.Type == "komorka").Value);
            //if (komorkaId != 39 && User.Claims.FirstOrDefault(u => u.Type.EndsWith("role")).Value != "3")
            //{
            //    kontrolaIQ = kontrolaIQ.Where(k => k.KomorkaWiodacaId == komorkaId);
            //}

            var list = await kontrolaIQ
                .Include(u => u.JednostkaKontrolujaca)
                .Include(u => u.JednostkaKontrolowana)
                .Include(u => u.KomorkaWiodaca)
                .Include(u => u.TypKontroli)
                .Include(u => u.Status)
                .ToListAsync();

            var viewList = _mapper.Map<List<KontrolaViewModel>>(list);

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Arkusz1");
                workSheet.Cells.LoadFromCollection(viewList, true, OfficeOpenXml.Table.TableStyles.Medium2);
                workSheet.Cells.AutoFitColumns();
                workSheet.Column(3).Style.Numberformat.Format = "yyyy-mm-dd";
                workSheet.Column(10).Style.Numberformat.Format = "yyyy-mm-dd";
                workSheet.Column(11).Style.Numberformat.Format = "yyyy-mm-dd";
                package.Save();
            }

            stream.Position = 0;

            string excelName = $"{nameof(Kontrole)}-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";

            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", nameof(Kontrola))
                .Warning("Pobranie listy kontroli zewnętrznych do pliku.");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}
