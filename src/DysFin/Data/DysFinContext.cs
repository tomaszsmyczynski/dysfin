using DysFin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DysFin.Data
{
    /// <summary>
    /// Kontekst bazy danych.
    /// </summary>
    public class DysFinContext : DbContext
    {
        public DysFinContext(DbContextOptions<DysFinContext> options) : base(options)
        {
        }

        public DbSet<JednostkaKontrolowana> JednostkaKontrolowana { get; set; }

        public DbSet<JednostkaKontrolujaca> JednostkaKontrolujaca { get; set; }

        public DbSet<Komorka> Komorka { get; set; }

        public DbSet<Kontrola> Kontrola { get; set; }

        public DbSet<KontrolaKomorki> KontrolaKomorki { get; set; }

        public DbSet<KontrolaProcesy> KontrolaProcesy { get; set; }

        public DbSet<Nieprawidlowosc> Nieprawidlowosc { get; set; }

        public DbSet<SlownikPoziomUzytkownika> SlownikPoziomUzytkownika { get; set; }

        public DbSet<SlownikProces> SlownikProces { get; set; }

        public DbSet<SlownikStatusKontroli> SlownikStatusKontroli { get; set; }

        public DbSet<SlownikTypKontroli> SlownikTypKontroli { get; set; }

        public DbSet<Uzytkownik> Uzytkownik { get; set; }

        public DbSet<Zalacznik> Zalacznik { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JednostkaKontrolowana>().ToTable(nameof(JednostkaKontrolowana));
            modelBuilder.Entity<JednostkaKontrolujaca>().ToTable(nameof(JednostkaKontrolujaca));
            modelBuilder.Entity<Komorka>().ToTable(nameof(Komorka));
            modelBuilder.Entity<Kontrola>().ToTable(nameof(Kontrola));
            modelBuilder.Entity<KontrolaKomorki>().ToTable(nameof(KontrolaKomorki));
            modelBuilder.Entity<KontrolaProcesy>().ToTable(nameof(KontrolaProcesy));
            modelBuilder.Entity<Nieprawidlowosc>().ToTable(nameof(Nieprawidlowosc));

            modelBuilder.Entity<SlownikPoziomUzytkownika>().ToTable(nameof(SlownikPoziomUzytkownika));
            modelBuilder.Entity<SlownikProces>().ToTable(nameof(SlownikProces));
            modelBuilder.Entity<SlownikStatusKontroli>().ToTable(nameof(SlownikStatusKontroli));
            modelBuilder.Entity<SlownikTypKontroli>().ToTable(nameof(SlownikTypKontroli));

            modelBuilder.Entity<Uzytkownik>().ToTable(nameof(Uzytkownik));
            modelBuilder.Entity<Zalacznik>().ToTable(nameof(Zalacznik));

            modelBuilder.Entity<KontrolaKomorki>()
                .HasKey(k => new { k.KontrolaId, k.KomorkaId });

            modelBuilder.Entity<KontrolaKomorki>()
                .HasOne(x => x.Komorka)
                .WithMany(y => y.KontrolaKomorki)
                .HasForeignKey(x => x.KomorkaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KontrolaProcesy>()
                .HasKey(k => new { k.KontrolaId, k.ProcesId });

            modelBuilder.Entity<KontrolaProcesy>()
                .HasOne(x => x.Proces)
                .WithMany(y => y.KontrolaProcesy)
                .HasForeignKey(x => x.ProcesId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
