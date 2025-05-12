using Microsoft.Data.SqlClient;
using WorkshopApp.Models;

namespace WorkshopApp.Repositories;

public class VisitsRepository : IVisitsRepository
{
    private readonly string _connectionString;

    public VisitsRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");

    }

    public async Task<Visit?> GetVisitByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT  * from visit where visit_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        await con.OpenAsync(cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync())
        {
            int visit_id = (int)reader["visit_id"];
            int client_id = (int)reader["client_id"];
            int mechanic_id = (int)reader["mechanic_id"];
            DateTime date = (DateTime)reader["date"];

            var visit = new Visit
            {
                visit_id = visit_id,
                client_id = client_id,
                mechanic_id = mechanic_id,
                date = date,
            };

            return visit;
        }

        return null;
    }
}