using System.Data;
using System.Diagnostics.Metrics;
using System.Text;

namespace CourrierFront.Models
{
    public class CourrierDataAccess
    {
        private readonly DatabaseManager _databaseManager;
        public CourrierDataAccess(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public List<Courrier> GetCourriers(string reference, string expediteur, string objet, int page, int pageSize)
        {
            string query = BuildQuery(reference, expediteur, objet, page, pageSize);
            DataTable dataTable = _databaseManager.ExecuteQuery(query);

            List<Courrier> courriers = MapDataTableToCourriers(dataTable);

            return courriers;
        }

        private string BuildQuery(string reference, string expediteur, string objet, int page, int pageSize)
        {
            StringBuilder queryBuilder = new StringBuilder("SELECT c.*, cd.Id IdCourrierDestinataire, " +
                "IdStatut, IdFlag, IdDepartement, s.Libelle StatutLibelle, f.Libelle FlagLibelle, " +
                "d.Nom DepartementNom " +
                "FROM Courriers c JOIN CourriersDestinataires cd ON(c.Id = cd.IdCourrier)" +
                " JOIN Statuts s ON(cd.IdStatut = s.Id) " +
                " JOIN Flags f ON(f.Id = c.IdFlag)" +
                " JOIN Departements d ON(cd.IdDepartement = d.Id) WHERE 1 = 1 ");

            AddSearchCriteria(ref queryBuilder, "Reference", reference);
            AddSearchCriteria(ref queryBuilder, "Expediteur", expediteur);
            AddSearchCriteria(ref queryBuilder, "Objet", objet);

            queryBuilder.Append($" ORDER BY Id OFFSET {(page - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY");

            return queryBuilder.ToString();
        }

        private void AddSearchCriteria(ref StringBuilder queryBuilder, string columnName, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                queryBuilder.Append($" AND {columnName} LIKE '%{value}%'");
            }
        }

        private List<Courrier> MapDataTableToCourriers(DataTable dataTable)
        {
            List<Courrier> courriers = new List<Courrier>();

            foreach (DataRow row in dataTable.Rows)
            {
                Courrier courrier = MapDataRowToCourrier(row);
                courriers.Add(courrier);
            }

            return courriers;
        }

        private Courrier MapDataRowToCourrier(DataRow row)
        {
            Courrier courrier = new Courrier
            {
                Id = Convert.ToInt32(row["Id"]),
                Reference = row["Reference"].ToString(),
                Expediteur = row["Expediteur"].ToString(),
                Objet = row["Objet"].ToString(),
                Commentaire = row["Commentaire"].ToString(),
                DateCreation = Convert.ToDateTime(row["DateCreation"]),
                Fichier = row["Fichier"].ToString(),
                Interne = Convert.ToBoolean(row["Interne"]),
                IdCourrierDestinataire = Convert.ToInt32(row["IdCourrierDestinataire"])
            };

            courrier.Departement = MapDepartement(row);
            courrier.Statut = MapStatut(row);
            courrier.Flag = MapFlag(row);

            return courrier;
        }

        private Departement MapDepartement(DataRow row)
        {
            return new Departement
            {
                Id = Convert.ToInt32(row["IdDepartement"]),
                Nom = row["DepartementNom"].ToString()
            };
        }

        private Statut MapStatut(DataRow row)
        {
            return new Statut
            {
                Id = Convert.ToInt32(row["IdStatut"]),
                Libelle = row["StatutLibelle"].ToString()
            };
        }

        private Flag MapFlag(DataRow row)
        {
            return new Flag
            {
                Id = Convert.ToInt32(row["IdFlag"]),
                Libelle = row["FlagLibelle"].ToString()
            };
        }

    }
}
