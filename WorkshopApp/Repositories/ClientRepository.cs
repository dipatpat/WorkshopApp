using Microsoft.Data.SqlClient;
using WorkshopApp.Models;

namespace WorkshopApp.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly string _connectionString;

    public ClientRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");

    }
    
    public async Task<Client?> GetClientByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT  * from client where client_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        await con.OpenAsync(cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync())
        {
            int client_id = (int)reader["client_id"];
            string first_name = (string)reader["first_name"];
            string last_name = (string)reader["last_name"];
            DateTime date_of_birth = (DateTime)reader["date_of_birth"];

            var visit = new Client
            {
                client_id = client_id,
                first_name = first_name,
                last_name = last_name,
                date_of_birth = date_of_birth,
            };

            return visit;
        }

        return null;
    }
}