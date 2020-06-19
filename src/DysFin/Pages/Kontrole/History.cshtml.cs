using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DysFin.Data;
using DysFin.Models;
using DysFin.Web.Models.DysFinViewModels;
using DysFin.Web.Models;
using System.Text.Json;

namespace DysFin.Web.Pages.Kontrole
{
    public class HistoryModel : PageModel
    {
        private readonly DysFinContext _context;

        public HistoryModel(DysFinContext context)
        {
            _context = context;
        }

        public HistoriaViewModel Historia = new HistoriaViewModel();

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["Id"] = id;
            Historia.Kontrola = _context.Kontrola.Find(id).Numer;

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = string.Format("SELECT * FROM [Historia] WHERE [MainRecordId] = {0} AND ([Table] = 'Kontrola' OR [Table] = 'Zalacznik' OR [Table] = 'Nieprawidlowosc')", id);
                _context.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var wpis = new WpisHistorii()
                            {
                                Data = reader.GetDateTime(3),
                                Autor = _context.Uzytkownik.Find(reader.GetInt32(6)).ImieNazwisko,
                                Wiadomosc = reader.GetString(1)
                            };

                            if (reader.GetString(7) == nameof(Kontrola))
                            {
                                wpis.KontrolaPrzed = reader.IsDBNull(10) ? null : JsonSerializer.Deserialize<Kontrola>(reader.GetString(10));
                                wpis.KontrolaPo = reader.IsDBNull(11) ? null : JsonSerializer.Deserialize<Kontrola>(reader.GetString(11));
                                if (wpis.KontrolaPrzed != null && wpis.KontrolaPo != null)
                                {
                                    wpis.KontrolaPrzed.Status = _context.SlownikStatusKontroli.Find(wpis.KontrolaPrzed.StatusId);
                                    wpis.KontrolaPo.Status = _context.SlownikStatusKontroli.Find(wpis.KontrolaPo.StatusId);
                                }
                            }
                            else
                            {
                                wpis.NieprawidlowoscPrzed = reader.IsDBNull(10) ? null : JsonSerializer.Deserialize<Nieprawidlowosc>(reader.GetString(10));
                                wpis.NieprawidlowoscPo = reader.IsDBNull(11) ? null : JsonSerializer.Deserialize<Nieprawidlowosc>(reader.GetString(11));
                            }

                            Historia.Wpisy.Add(wpis);
                        }
                    }
                }
            }

            return Page();
        }
    }
}
