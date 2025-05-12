using Microsoft.Data.SqlClient;
using WorkshopApp.Models;

namespace WorkshopApp.Repositories;

public class MechanicRepository : IMechanicRepository
{
    private readonly string _connectionString;

    public MechanicRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");

    }

    public async Task<Mechanic?> GetMechanicByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT  * from mechanic where mechanic_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        await con.OpenAsync(cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync())
        {
            int mechanic_id = (int)reader["mechanic_id"];
            string first_name = (string)reader["first_name"];
            string last_name = (string)reader["last_name"];
            string licence_number = (string)reader["licence_number"];

            var mechanic = new Mechanic
            {
                mechanic_id = mechanic_id,
                first_name = first_name,
                last_name = last_name,
                license_number = licence_number, 
            };

            return mechanic;
        }

        return null;
    }
}
