using System.ComponentModel.DataAnnotations.Schema;

namespace Courrier.Models.Courrier
{
    public class StatutCourrier
    {

        public int Id { get; set; }
        [ForeignKey("Statut")]
        public int IdStatut { get; set; }
        [ForeignKey("Courrier")]
        public int IdCourrier { get; set; }
        public DateTime Date { get; set; }

        public Statut Statut { get; set; }
        public CourrierModel Courrier { get; set; }
    }
}
