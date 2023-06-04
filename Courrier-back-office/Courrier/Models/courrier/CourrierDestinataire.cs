using Courrier.Models.courrier;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courrier.Models.Courrier
{
    public class CourrierDestinataire
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [ForeignKey("Departement")]
        [Required]
        public int IdDepartement { get; set; }
        [ForeignKey("Courrier")]
        public int IdCourrier { get; set; }
        [ForeignKey("Statut")]
        public int IdStatut { get; set; }

        [ForeignKey("User")]
        public int? IdCoursier { get; set; }

        public Departement Departement { get; set; }
        public CourrierModel Courrier { get; set; }
        public User? Coursier { get; set; }
        public Statut Statut { get; set; }
        public ICollection<HistoriqueCourrierDestinataire> Historiques { get; set; }

    }
}
