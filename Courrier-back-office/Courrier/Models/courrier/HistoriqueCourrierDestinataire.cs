using Courrier.Models.Courrier;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courrier.Models.courrier
{
    public class HistoriqueCourrierDestinataire
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [ForeignKey("CourrierDestinataire")]
        public int IdCourrierDestinataire { get; set; }
        public CourrierDestinataire CourrierDestinaire { get; set; }

        [ForeignKey("StatutCourrier")]
        public int IdStatut { get; set; }
        public Statut Statut { get; set; }
        public DateTime DateHistorique { get; set; } = DateTime.UtcNow;

    }
}
