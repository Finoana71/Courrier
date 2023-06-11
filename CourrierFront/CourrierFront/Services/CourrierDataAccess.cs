using CourrierFront.Models;
using Microsoft.Extensions.ObjectPool;
using System.Data;
using System.Diagnostics.Metrics;
using System.Security.Cryptography.Xml;
using System.Text;

namespace CourrierFront.Services
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

        public List<Courrier> GetCourriersNoPaginate(string reference, string expediteur, string objet)
        {
            string query = BuildQueryNoPaginate(reference, expediteur, objet).ToString();
            DataTable dataTable = _databaseManager.ExecuteQuery(query);

            List<Courrier> courriers = MapDataTableToCourriers(dataTable);

            return courriers;
        }


        public StringBuilder BuildQueryNoPaginate(string reference, string expediteur, string objet)
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
            return queryBuilder;
        }

        private string BuildQuery(string reference, string expediteur, string objet, int page, int pageSize)
        {
            if (pageSize <= 0)
                pageSize = 10;
            if (page < 1)
                page = 1;

            StringBuilder queryBuilder = BuildQueryNoPaginate(reference, expediteur, objet);
            queryBuilder.Append($" ORDER BY Id OFFSET {(page - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY");

            return queryBuilder.ToString();
        }

        private string BuildCountQuery(string reference, string expediteur, string objet)
        {
            StringBuilder queryBuilder = new StringBuilder("SELECT COUNT(*) FROM Courriers c JOIN CourriersDestinataires cd ON(c.Id = cd.IdCourrier)" +
                " JOIN Statuts s ON(cd.IdStatut = s.Id) " +
                " JOIN Flags f ON(f.Id = c.IdFlag)" +
                " JOIN Departements d ON(cd.IdDepartement = d.Id) WHERE 1 = 1 ");

            AddSearchCriteria(ref queryBuilder, "Reference", reference);
            AddSearchCriteria(ref queryBuilder, "Expediteur", expediteur);
            AddSearchCriteria(ref queryBuilder, "Objet", objet);

            return queryBuilder.ToString();
        }


        public int GetTotalCourriersCount(string reference, string expediteur, string objet)
        {
            string query = BuildCountQuery(reference, expediteur, objet);
            object result = _databaseManager.ExecuteScalar(query);

            int totalCount = 0;
            if (result != null && int.TryParse(result.ToString(), out int count))
            {
                totalCount = count;
            }
            return totalCount;
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

            if (dataTable != null && dataTable.Rows != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    Courrier courrier = MapDataRowToCourrier(row);
                    courriers.Add(courrier);
                }
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
        private Departement MapDepartementList(DataRow row)
        {
            return new Departement
            {
                Id = Convert.ToInt32(row["Id"]),
                Nom = row["Nom"].ToString()
            };
        }

        private Statut MapStatutList(DataRow row)
        {
            return new Statut
            {
                Id = Convert.ToInt32(row["Id"]),
                Libelle = row["Libelle"].ToString()
            };
        }

        private Flag MapFlagList(DataRow row)
        {
            return new Flag
            {
                Id = Convert.ToInt32(row["Id"]),
                Libelle = row["Libelle"].ToString()
            };
        }

        public List<Departement> GetDepartements()
        {
            string query = "SELECT * FROM departements";
            DataTable dataTable = _databaseManager.ExecuteQuery(query);

            List<Departement> list = MapDataTableToDepartements(dataTable);

            return list;
        }
        private List<Departement> MapDataTableToDepartements(DataTable dataTable)
        {
            List<Departement> list= new List<Departement>();

            if (dataTable != null && dataTable.Rows != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    Departement data = MapDepartementList(row);
                    list.Add(data);
                }
            }
            return list;
        }
        public List<Flag> GetFlags()
        {
            string query = "SELECT * FROM flags";
            DataTable dataTable = _databaseManager.ExecuteQuery(query);

            List<Flag> list = MapDataTableToFlags(dataTable);

            return list;
        }
        private List<Flag> MapDataTableToFlags(DataTable dataTable)
        {
            List<Flag> list = new List<Flag>();

            if (dataTable != null && dataTable.Rows != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    Flag data = MapFlagList(row);
                    list.Add(data);
                }
            }
            return list;
        }

        public List<Statut> GetStatuts()
        {
            string query = "SELECT * FROM statuts";
            DataTable dataTable = _databaseManager.ExecuteQuery(query);

            List<Statut> list = MapDataTableToStatuts(dataTable);

            return list;
        }
        private List<Statut> MapDataTableToStatuts(DataTable dataTable)
        {
            List<Statut> list = new List<Statut>();

            if (dataTable != null && dataTable.Rows != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    Statut data = MapStatutList(row);
                    list.Add(data);
                }
            }
            return list;
        }

    }
}
