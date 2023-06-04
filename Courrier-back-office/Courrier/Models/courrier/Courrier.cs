using Courrier.Models.courrier;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courrier.Models.Courrier
{
    public class CourrierModel
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Reference { get; set; }
        public string Expediteur { get; set; }
        public string Objet { get; set; }
        public string? Commentaire { get; set; }
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
        public string? Fichier { get; set; }
        public Boolean Interne { get ; set; }

        [ForeignKey("User")]
        public int IdReceptionniste { get; set; }
        public User Receptionniste { get; set; }

        public ICollection<CourrierDestinataire> Destinataires { get; set; }


        [ForeignKey("Flag")]
        public int IdFlag{ get; set; }
        public Flag Flag{ get; set; }
    }
}
