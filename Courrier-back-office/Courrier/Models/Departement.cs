using Courrier.Models.Courrier;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courrier.Models
{
    public class Departement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Nom { get; set; }

        public ICollection<CourrierDestinataire> CourrierDestinataires { get; set; }

    }
}
