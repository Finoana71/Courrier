namespace CourrierFront.Models
{
    public class CourrierViewModel
    {
        public string Reference { get; set; }
        public string Expediteur { get; set; }
        public string Objet { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; } = 2;
        public List<Courrier> Courriers { get; set; }
        public List<Departement> Departements { get; set; }
        public List<Flag> Flags { get; set; }
        public List<Statut> Statuts { get; set; }
        public int TotalCount { get; set; }
        public int PageCount => (int)Math.Ceiling((double)TotalCount / PageSize);

        public int IdFlag { get; set; }
        public int IdDepartement { get; set; }
        public int IdStatut { get; set; }

    }
}
