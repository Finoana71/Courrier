using System.ComponentModel.DataAnnotations.Schema;

namespace Courrier.Models
{
    public class User
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id{ get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [ForeignKey("Role")]
        public int IdRole { get; set; }
        [ForeignKey("Departement")]
        public int? IdDepartement{ get; set; }
        public virtual Role Role { get; set; }
        public virtual Departement? Departement { get; set; }
    }
}
