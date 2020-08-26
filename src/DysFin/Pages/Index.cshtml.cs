using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DysFin.Data;
using DysFin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DysFin.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DysFinContext _context;

        public IndexModel(ILogger<IndexModel> logger, DysFinContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public int KontrolaDzialaniaNaprawcze { get; set; }

        [BindProperty]
        public int KontrolaDoOceny { get; set; }

        public Dictionary<string, int> KontrolaProcesy = new Dictionary<string, int>();

        public Dictionary<KeyValuePair<int,string>, int> KontrolaJednostki = new Dictionary<KeyValuePair<int, string>, int>();

        public Dictionary<string, int> ZaleceniaJednostki = new Dictionary<string, int>();

        public List<SlownikStatusKontroli> KontrolaStatusy = new List<SlownikStatusKontroli>();

        public void OnGet()
        {
            var komorka = User.Claims.FirstOrDefault(u => u.Type == "komorka").Value;

            KontrolaDzialaniaNaprawcze = _context.Kontrola.Count(k => k.KomorkaWiodacaId == int.Parse(komorka) && k.StatusId == 43);
            KontrolaDoOceny = _context.Kontrola.Count(k => k.StatusId == 40);

            foreach(var proces in _context.SlownikProces)
            {
                KontrolaProcesy.Add(proces.Nazwa, _context.Kontrola.Where(k => k.Procesy.Any(p => p.ProcesId == proces.Id)).Count());
            }

            foreach (var jednostka in _context.Kontrola.Include(k => k.JednostkaKontrolujaca).AsEnumerable().GroupBy(k => k.JednostkaKontrolujaca))
            {
                KontrolaJednostki.Add(new KeyValuePair<int, string>(jednostka.Key.Id, jednostka.Key.Nazwa), jednostka.Count());
            }

            foreach (var jednostka in _context.Nieprawidlowosc.Include(n => n.Kontrola).Include(j => j.Kontrola.JednostkaKontrolujaca).AsEnumerable().GroupBy(k => k.Kontrola.JednostkaKontrolujaca))
            {
                var nierozwiazaneZalecenia = jednostka.Count(j => j.DataZakonczeniaDzialan.HasValue == false);
                if (nierozwiazaneZalecenia > 0)
                {
                    ZaleceniaJednostki.Add(jednostka.Key.Nazwa, nierozwiazaneZalecenia);
                }
            }
        }

        public JsonResult OnGetKontrolaStatusy()
        {
            IEnumerable<IGrouping<SlownikStatusKontroli,Kontrola>> kontrole;

            if (User.Claims.FirstOrDefault(u => u.Type.EndsWith("role")).Value == "2")
            {
                kontrole = _context.Kontrola.Include(k => k.Status).Where(k => k.StatusId != 41).AsEnumerable().GroupBy(k => k.Status);
            }
            else
            {
                var komorka = User.Claims.FirstOrDefault(u => u.Type == "komorka").Value;
                kontrole = _context.Kontrola.Include(k => k.Status).Where(k => k.StatusId != 41 && k.KomorkaWiodacaId == int.Parse(komorka)).AsEnumerable().GroupBy(k => k.Status);
            }

            foreach (var status in kontrole)
            {
                KontrolaStatusy.Add(new SlownikStatusKontroli()
                {
                    Id = status.Count(),
                    Symbol = 's' + status.Key.Id.ToString(),
                    Nazwa = status.Key.Nazwa
                });
            }

            return new JsonResult(KontrolaStatusy);
        }
    }
}
