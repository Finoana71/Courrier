using System.ComponentModel.DataAnnotations.Schema;

namespace Courrier.Models
{
    public class User
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id{ get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public virtual Role role { get; set; }
        public virtual Departement departement { get; set; }
    }
}
