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
    /// Model danych użytkownika.
    /// </summary>
    public class Uzytkownik
    {
        /// <summary>
        /// Identyfikator.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Login.
        /// </summary>
        [Display(Name = "Login")]
        [Required]
        public string Login { get; set; }

        /// <summary>
        /// Poziom użytkownika.
        /// </summary>
        [Display(Name = "Poziom")]
        public int PoziomUzytkownikaId { get; set; }

        [ForeignKey("PoziomUzytkownikaId"), Display(Name = "Poziom")]
        public SlownikPoziomUzytkownika? PoziomUzytkownika { get; set; }

        /// <summary>
        /// Status aktywności konta.
        /// </summary>
        [Display(Name = "Status")]
        public bool Status { get; set; }

        /// <summary>
        /// Imię.
        /// </summary>
        [Display(Name = "Imię")]
        [Required]
        public string Imie { get; set; }

        /// <summary>
        /// Nazwisko.
        /// </summary>
        [Display(Name = "Nazwisko")]
        [Required]
        public string Nazwisko { get; set; }

        /// <summary>
        /// Imię i nazwisko.
        /// </summary>
        public string ImieNazwisko
        {
            get
            {
                return Imie + ' ' + Nazwisko;
            }
        }

        /// <summary>
        /// Numer telefonu.
        /// </summary>
        [Display(Name = "Telefon")]
        public string? Telefon { get; set; }

        /// <summary>
        /// Adres e-mail.
        /// </summary>
        [Display(Name = "E-mail")]
        public string? Email { get; set; }

        /// <summary>
        /// Opcjonalny identyfikator przynależności do jednostki kontrolowanej.
        /// </summary>
        [Display(Name = "Jednostka")]
        public int? JednostkaId { get; set; }

        [ForeignKey("JednostkaId")]
        public JednostkaKontrolowana? Jednostka { get; set; }

        /// <summary>
        /// Opcjonalny identyfikator przynależności do kontrolowanej komórki.
        /// </summary>
        [Display(Name = "Komórka kontrolowana")]
        public int? KomorkaId { get; set; }

        public Komorka? Komorka { get; set; }
    }
}
