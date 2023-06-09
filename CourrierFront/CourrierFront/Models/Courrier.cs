namespace CourrierFront.Models
{
    public class Courrier
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public string Expediteur { get; set; }
        public string Objet { get; set; }
        public string? Commentaire { get; set; }
        public DateTime DateCreation { get; set; }
        public string? Fichier { get; set; }
        public Boolean Interne { get; set; }
        public int IdCourrierDestinataire { get; set; }

        public Departement Departement { get; set; }
        public Statut Statut { get; set; }
        public Flag Flag { get; set; }

    }
}
