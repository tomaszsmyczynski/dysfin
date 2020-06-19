using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DysFin.Models.DysFinViewModels
{
    public class UzytkownikViewModel
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string PoziomUzytkownikaNazwa { get; set; }

        public bool Status { get; set; }

        public string Imie { get; set; }

        public string Nazwisko { get; set; }

        public string Telefon { get; set; }

        public string Email { get; set; }

        //public string JednostkaNazwa { get; set; }

        public string KomorkaNazwa { get; set; }
    }
}
