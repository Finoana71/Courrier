using System.Data.SqlClient;
using System.Data;

namespace CourrierFront.Services
{
    public class DatabaseManager
    {
        private readonly string connectionString;


        public DatabaseManager(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("SqlServer");
        }

        public void ExecuteNonQuery(string query)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'exécution de la requête : {ex.Message}");
                }
            }
        }
        public object ExecuteScalar(string query)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    return command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'exécution de la requête : {ex.Message}");
                    return null;
                }
            }
        }

        public DataTable ExecuteQuery(string query)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    return dataTable;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'exécution de la requête : {ex.Message}");
                    return null;
                }
            }
        }
    }
}
