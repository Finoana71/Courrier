using Courrier.Models;
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

        public void SeedData()
        {
            var roles = new List<Role>
            {
                new Role{Id=1, nom="Directeur" },
                new Role{Id=2, nom="Récéptionniste" },
                new Role{Id=3, nom="Coursier" }
            };
            roles.ForEach(r => Roles.Add(r));
            var departements = new List<Departement>
            {
                new Departement{Id=1, nom="Ressources Humaines"},
                new Departement{Id=2, nom="Marketing"}
            };
            departements.ForEach(r => Departements.Add(r));
            var users = new List<User>
            {
                new User{email="rakoto@gmail.com", password=BCryptNet.HashPassword("12345678"), username="Rakoto", role=roles[0], departement=departements[1]},
                new User{email="rabe@gmail.com", password=BCryptNet.HashPassword("12345678"), username="Rabe", role=roles[1], departement=departements[1]}
            };
            users.ForEach(r => Users.Add(r));
            SaveChanges();

        }
    }
}
