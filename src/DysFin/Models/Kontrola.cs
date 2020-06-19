using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

#nullable enable
namespace DysFin.Models
{
    /// <summary>
    /// Model danych kontroli zewnętrznej.
    /// </summary>
    public class Kontrola
    {
        /// <summary>
        /// Identyfikator.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Kolejny numer w roku.
        /// </summary>
        public int Lp { get; set; }

        /// <summary>
        /// Rok.
        /// </summary>
        public int Rok { get; set; }

        /// <summary>
        /// Numer.
        /// </summary>
        [Display(Name = "Numer")]
        public string Numer
        {
            get
            {
                return Lp.ToString() + '/' + Rok.ToString();
            }
        }

        /// <summary>
        /// Znak akt w komórce wiodącej (dotyczy tylko kontroli w Urzędzie m.st. Warszawy)
        /// </summary>
        [Display(Name = "Znak akt w komórce wiodącej")]
        public string? ZnakWiodacy { get; set; }

        /// <summary>
        /// Data wpisu do rejestru
        /// </summary>
        [Display(Name = "Data wpisu")]
        [DataType(DataType.Date)]
        public DateTime DataWpisu { get; set; }

        /// <summary>
        /// Identyfikator jednostki kontrolującej.
        /// </summary>
        public int JednostkaKontrolujacaId { get; set; }

        [Display(Name = "Jednostka kontrolująca")]
        public JednostkaKontrolujaca JednostkaKontrolujaca { get; set; }

        /// <summary>
        /// Dane kontrolera / kontrolerów
        /// </summary>
        [Display(Name = "Imię, nazwisko, stanowisko kontrolera / kontrolerów")]
        [Required(AllowEmptyStrings = false)]
        public string Kontroler { get; set; }

        /// <summary>
        /// Data upoważnienia.
        /// </summary>
        [Display(Name = "Data dokumentu upoważnienia")]
        [DataType(DataType.Date)]
        public DateTime DataUpowaznienia { get; set; }

        /// <summary>
        /// Numer upoważnienia.
        /// </summary>
        [Display(Name = "Numer dokumentu upoważnienia")]
        [Required(AllowEmptyStrings = false)]
        public string NumerUpowaznienia { get; set; }

        /// <summary>
        /// Temat.
        /// </summary>
        [Display(Name = "Temat kontroli")]
        [Required(AllowEmptyStrings = false)]
        public string Temat { get; set; }

        /// <summary>
        /// Początek okresu objętego kontrolą.
        /// </summary>
        [Display(Name = "Okres kontroli - start")]
        [DataType(DataType.Date)]
        public DateTime OkresOd { get; set; }

        /// <summary>
        /// Koniec okresu objętego kontrolą.
        /// </summary>
        [Display(Name = "Okres kontroli - koniec")]
        [DataType(DataType.Date)]
        public DateTime OkresDo { get; set; }

        /// <summary>
        /// Identyfikator jednostki kontrolowanej.
        /// </summary>
        public int? JednostkaKontrolowanaId { get; set; }

        [Display(Name = "Jednostka kontrolowana")]
        public JednostkaKontrolowana? JednostkaKontrolowana { get; set; }

        /// <summary>
        /// Identyfikator komórki wiodącej.
        /// </summary>
        public int KomorkaWiodacaId { get; set; }

        [Display(Name = "Komórka wiodąca")]
        public Komorka? KomorkaWiodaca { get; set; }

        /// <summary>
        /// Lista komorek uczestniczących.
        /// </summary>
        [Display(Name = "Komórki uczestniczące")]
        public ICollection<KontrolaKomorki> KomorkiUczestniczace { get; } = new List<KontrolaKomorki>();

        /// <summary>
        /// Data rozpoczęcia kontroli.
        /// </summary>
        [Display(Name = "Data rozpoczęcia")]
        [DataType(DataType.Date)]
        public DateTime DataRozpoczecia { get; set; }

        /// <summary>
        /// Data zakończenia kontroli.
        /// </summary>
        [Display(Name = "Data zakończenia")]
        [DataType(DataType.Date)]
        public DateTime? DataZakonczenia { get; set; }

        /// <summary>
        /// Uwagi osoby dodającej kontrolę.
        /// </summary>
        [Display(Name = "Uwagi wpisującego")]
        public string? Uwagi { get; set; }

        /// <summary>
        /// Uwagi od Biura Kontroli.
        /// </summary>
        [Display(Name = "Uwagi Biura Kontroli")]
        public string? UwagiKW { get; set; }

        /// <summary>
        /// Identyfikator typu kontroli.
        /// </summary>
        public int TypKontroliId { get; set; }

        [ForeignKey("TypKontroliId")]
        [Display(Name = "Typ kontroli")]
        public SlownikTypKontroli? TypKontroli { get; set; }

        /// <summary>
        /// Identyfikator statusu.
        /// </summary>
        public int StatusId { get; set; }

        [ForeignKey("StatusId")]
        [Display(Name = "Status")]
        public SlownikStatusKontroli? Status { get; set; }

        /// <summary>
        /// Informacja czy kontrola dotyczy spraw kadrowych w Urzędzie Dzielnicy.
        /// </summary>
        [Display(Name = "Sprawy kadrowe")]
        public bool SprawyKadrowe { get; set; }

        /// <summary>
        /// Informacja czy kontrola dotyczy wykorzystania funduszy europejskich.
        /// </summary>
        [Display(Name = "Wykorzystanie funduszy UE")]
        public bool FunduszeUE { get; set; }

        /// <summary>
        /// Lista załączników.
        /// </summary>
        [Display(Name = "Lista załączników")]
        public ICollection<Zalacznik>? Zalaczniki { get; set; }

        /// <summary>
        /// Kolekcja nieprawidłowości/zaleceń.
        /// </summary>
        [Display(Name = "Nieprawidłowości/zalecenia")]
        public ICollection<Nieprawidlowosc>? Nieprawidlowosci { get; set; }

        /// <summary>
        /// Kolekcja procesów.
        /// </summary>
        [Display(Name = "Procesy kontrolowane")]
        public ICollection<KontrolaProcesy>? Procesy { get; } = new List<KontrolaProcesy>();
    }
}
