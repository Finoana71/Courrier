using Courrier.Models;
using Courrier.Models.courrier;
using Courrier.Models.Courrier;
using Microsoft.EntityFrameworkCore;
using BCryptNet = BCrypt.Net.BCrypt;


namespace Courrier.DAL
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=courrier;Integrated Security=True");
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Departement> Departements { get; set; }

        public DbSet<Statut> Statuts { get; set; }
        public DbSet<Flag> Flags { get; set; }
        public DbSet<CourrierModel> Courriers { get; set; }
        public DbSet<CourrierDestinataire> CourriersDestinataires { get; set; }
        public DbSet<StatutCourrier> StatutsCourrier { get; set; }
        public DbSet<HistoriqueCourrierDestinataire> HistoriqueCourrierDestinataire { get; set; }

        public void SeedData()
        {
            var roles = new List<Role>
            {
                new Role{Id=1, Nom="Directeur" },
                new Role{Id=2, Nom="Secretaire" },
                new Role{Id=3, Nom="Récéptionniste" },
                new Role{Id=4, Nom="Coursier" }
            };
            roles.ForEach(r => Roles.Add(r));
            var flags = new List<Flag>
            {
                new Flag{Libelle="Important" },
                new Flag{Libelle="Non important" },
            };
            flags.ForEach(r => Flags.Add(r));

            var departements = new List<Departement>
            {
                new Departement{Id=1, Nom="Ressources Humaines"},
                new Departement{Id=2, Nom="Marketing"}
            };
            departements.ForEach(r => Departements.Add(r));
            var users = new List<User>
            {
                new User{Email="receptionniste@gmail.com", Password=BCryptNet.HashPassword("12345678"), Username="Receptionniste", IdRole=3},
                new User{Email="coursier@gmail.com", Password=BCryptNet.HashPassword("12345678"), Username="Coursier", IdRole=4},
                new User{Email="secretaire1@gmail.com", Password=BCryptNet.HashPassword("12345678"), Username="Secretaire 1", IdRole=2, IdDepartement=1},
                new User{Email="secretaire2@gmail.com", Password=BCryptNet.HashPassword("12345678"), Username="Secretaire 2", IdRole=2, IdDepartement=2},
                new User{Email="directeur1@gmail.com", Password=BCryptNet.HashPassword("12345678"), Username="Directeur 1", IdRole=1, IdDepartement=1},
                new User{Email="directeur2@gmail.com", Password=BCryptNet.HashPassword("12345678"), Username="Directeur 1", IdRole=1, IdDepartement=1}
            };
            users.ForEach(r => Users.Add(r));
            var statuts = new List<Statut>
            {
                new Statut{Id=1, Libelle="Reçu"},
                new Statut{Id=2, Libelle="Transféré coursier"},
                new Statut{Id=3, Libelle="Transféré secretaire"},
                new Statut{Id=4, Libelle="Livré"}
            };
            statuts.ForEach(r => Statuts.Add(r));

            SaveChanges();

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Courrier
            modelBuilder.Entity<CourrierModel>()
                .HasOne(e => e.Receptionniste)
                .WithMany()
                .HasForeignKey(cd => cd.IdReceptionniste);


            modelBuilder.Entity<User>()
                .HasOne(e => e.Role);

            modelBuilder.Entity<User>()
                .HasOne(e => e.Departement);


            modelBuilder.Entity<CourrierDestinataire>()
                .HasOne(cd => cd.Courrier)
                .WithMany(c => c.Destinataires)
                .HasForeignKey(cd => cd.IdCourrier)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourrierDestinataire>()
                .HasOne(cd => cd.Departement)
                .WithMany(d => d.CourrierDestinataires)
                .HasForeignKey(cd => cd.IdDepartement)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CourrierDestinataire>()
                .HasOne(cd => cd.Coursier)
                .WithMany()
                .HasForeignKey(cd => cd.IdCoursier)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CourrierDestinataire>()
                .HasOne(cd => cd.Statut)
                .WithMany()
                .HasForeignKey(cd => cd.IdStatut)
                .OnDelete(DeleteBehavior.NoAction);


            //modelBuilder.Entity<CourrierModel>()
            //    .HasOne(cd => cd.Flag)
            //    .WithMany()
            //    .HasForeignKey(cd => cd.IdFlag)
            //    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CourrierModel>()
                .HasOne(e => e.Flag)
                .WithMany()
                .HasForeignKey(cd => cd.IdFlag);

            modelBuilder.Entity<StatutCourrier>()
                .HasOne(st => st.Statut)
                .WithMany()
                .HasForeignKey(cd => cd.IdStatut)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StatutCourrier>()
                .HasOne(st => st.Courrier)
                .WithMany()
                .HasForeignKey(cd => cd.IdCourrier)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<HistoriqueCourrierDestinataire>()
                .HasOne(st => st.Statut)
                .WithMany()
                .HasForeignKey(cd => cd.IdStatut)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HistoriqueCourrierDestinataire>()
                .HasOne(st => st.CourrierDestinaire)
                .WithMany()
                .HasForeignKey(cd => cd.IdCourrierDestinataire)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
