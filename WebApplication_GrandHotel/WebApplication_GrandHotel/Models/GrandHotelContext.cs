using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApplication_GrandHotel.Models
{
    public partial class GrandHotelContext : DbContext
    {
        public virtual DbSet<Adresse> Adresse { get; set; }
        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<Calendrier> Calendrier { get; set; }
        public virtual DbSet<Chambre> Chambre { get; set; }
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<Facture> Facture { get; set; }
        public virtual DbSet<LigneFacture> LigneFacture { get; set; }
        public virtual DbSet<ModePaiement> ModePaiement { get; set; }
        public virtual DbSet<Reservation> Reservation { get; set; }
        public virtual DbSet<Tarif> Tarif { get; set; }
        public virtual DbSet<TarifChambre> TarifChambre { get; set; }
        public virtual DbSet<Telephone> Telephone { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=GrandHotel;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Adresse>(entity =>
            {
                entity.HasKey(e => e.IdClient);

                entity.Property(e => e.IdClient).ValueGeneratedNever();

                entity.Property(e => e.CodePostal)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Complement).HasMaxLength(40);

                entity.Property(e => e.Rue)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.Ville)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.HasOne(d => d.IdClientNavigation)
                    .WithOne(p => p.Adresse)
                    .HasForeignKey<Adresse>(d => d.IdClient)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Adresse_Client");
            });

            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.HasIndex(e => e.UserId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
            });

            modelBuilder.Entity<Calendrier>(entity =>
            {
                entity.HasKey(e => e.Jour);

                entity.Property(e => e.Jour).HasColumnType("date");
            });

            modelBuilder.Entity<Chambre>(entity =>
            {
                entity.HasKey(e => e.Numero);

                entity.Property(e => e.Numero).ValueGeneratedNever();

                entity.Property(e => e.Douche).HasDefaultValueSql("((1))");

                entity.Property(e => e.Wc)
                    .HasColumnName("WC")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.Civilite)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.Nom)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.Prenom)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.Societe).HasMaxLength(100);
            });

            modelBuilder.Entity<Facture>(entity =>
            {
                entity.Property(e => e.CodeModePaiement)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.DateFacture).HasColumnType("date");

                entity.Property(e => e.DatePaiement).HasColumnType("date");

                entity.HasOne(d => d.CodeModePaiementNavigation)
                    .WithMany(p => p.Facture)
                    .HasForeignKey(d => d.CodeModePaiement)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Facture_Paiement");

                entity.HasOne(d => d.IdClientNavigation)
                    .WithMany(p => p.Facture)
                    .HasForeignKey(d => d.IdClient)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Facture_Client");
            });

            modelBuilder.Entity<LigneFacture>(entity =>
            {
                entity.HasKey(e => new { e.IdFacture, e.NumLigne });

                entity.Property(e => e.MontantHt)
                    .HasColumnName("MontantHT")
                    .HasColumnType("decimal(12, 3)");

                entity.Property(e => e.Quantite).HasDefaultValueSql("((1))");

                entity.Property(e => e.TauxReduction)
                    .HasColumnType("decimal(6, 3)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.TauxTva)
                    .HasColumnName("TauxTVA")
                    .HasColumnType("decimal(6, 3)");

                entity.HasOne(d => d.IdFactureNavigation)
                    .WithMany(p => p.LigneFacture)
                    .HasForeignKey(d => d.IdFacture)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LigneFacture_Facture");
            });

            modelBuilder.Entity<ModePaiement>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(40);
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => new { e.NumChambre, e.Jour });

                entity.HasIndex(e => e.IdClient)
                    .HasName("IDX_ReservationClient_FK");

                entity.Property(e => e.Jour).HasColumnType("date");

                entity.Property(e => e.HeureArrivee).HasDefaultValueSql("((17))");

                entity.HasOne(d => d.IdClientNavigation)
                    .WithMany(p => p.Reservation)
                    .HasForeignKey(d => d.IdClient)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reservation_Client");

                entity.HasOne(d => d.JourNavigation)
                    .WithMany(p => p.Reservation)
                    .HasForeignKey(d => d.Jour)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reservation_Calendrier");

                entity.HasOne(d => d.NumChambreNavigation)
                    .WithMany(p => p.Reservation)
                    .HasForeignKey(d => d.NumChambre)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reservation_Chambre");
            });

            modelBuilder.Entity<Tarif>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.DateDebut).HasColumnType("date");

                entity.Property(e => e.Prix).HasColumnType("decimal(12, 3)");
            });

            modelBuilder.Entity<TarifChambre>(entity =>
            {
                entity.HasKey(e => new { e.NumChambre, e.CodeTarif });

                entity.Property(e => e.CodeTarif)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.CodeTarifNavigation)
                    .WithMany(p => p.TarifChambre)
                    .HasForeignKey(d => d.CodeTarif)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TarifChambre_Tarif");

                entity.HasOne(d => d.NumChambreNavigation)
                    .WithMany(p => p.TarifChambre)
                    .HasForeignKey(d => d.NumChambre)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TarifChambre_Chambre");
            });

            modelBuilder.Entity<Telephone>(entity =>
            {
                entity.HasKey(e => e.Numero);

                entity.Property(e => e.Numero)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.CodeType)
                    .IsRequired()
                    .HasColumnType("char(1)");

                entity.HasOne(d => d.IdClientNavigation)
                    .WithMany(p => p.Telephone)
                    .HasForeignKey(d => d.IdClient)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Telephone_Client");
            });
        }
    }
}
