using System.ComponentModel.DataAnnotations.Schema;

namespace Courrier.Models
{
    public class Role
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string nom { get; set; }

    }
}
