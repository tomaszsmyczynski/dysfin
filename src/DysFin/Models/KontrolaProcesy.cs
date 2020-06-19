namespace DysFin.Models
{
    /// <summary>
    /// Model danych listy procesów kontroli.
    /// </summary>
    public class KontrolaProcesy
    {
        /// <summary>
        /// Identyfikator kontroli.
        /// </summary>
        public int KontrolaId { get; set; }

        public Kontrola Kontrola { get; set; }

        /// <summary>
        /// Identyfikator procesu.
        /// </summary>
        public int ProcesId { get; set; }

        public SlownikProces Proces { get; set; }
    }
}