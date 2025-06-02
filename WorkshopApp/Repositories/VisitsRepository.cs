using Microsoft.Data.SqlClient;
using WorkshopApp.DTOs;
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

    public Task<IEnumerable<string>> GetVisitsServices(int id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<VisitDto?> GetFullVisitByIdAsync(int visitId, CancellationToken cancellationToken)
{
    await using var con = new SqlConnection(_connectionString);
    await using var cmd = new SqlCommand();
    cmd.Connection = con;
    cmd.CommandText = @"
        SELECT 
            v.visit_id,
            v.date,
            c.first_name AS client_first_name,
            c.last_name AS client_last_name,
            c.date_of_birth AS client_dob,
            m.mechanic_id,
            m.licence_number,
            s.name AS service_name,
            vs.service_fee
        FROM visit v
        JOIN client c ON v.client_id = c.client_id
        JOIN mechanic m ON v.mechanic_id = m.mechanic_id
        JOIN visit_service vs ON v.visit_id = vs.visit_id
        JOIN service s ON vs.service_id = s.service_id
        WHERE v.visit_id = @id";
    cmd.Parameters.AddWithValue("@id", visitId);

    await con.OpenAsync(cancellationToken);
    cancellationToken.ThrowIfCancellationRequested();

    var reader = await cmd.ExecuteReaderAsync(cancellationToken);
    VisitDto? visitDto = null;

    while (await reader.ReadAsync(cancellationToken))
    {
        if (visitDto == null)
        {
            visitDto = new VisitDto
            {
                date = (DateTime)reader["date"],
                client = new ClientDto
                {
                    first_name = (string)reader["client_first_name"],
                    last_name = (string)reader["client_last_name"],
                    date_of_birth = (DateTime)reader["client_dob"]
                },
                mechanic = new MechanicDto
                {
                    mechanic_id = (int)reader["mechanic_id"],
                    license_number = (string)reader["licence_number"]
                },
                visitServices = new List<VisitServicesDto>()
            };
        }

        visitDto.visitServices.Add(new VisitServicesDto
        {
            name = (string)reader["service_name"],
            service_fee = (decimal)reader["service_fee"]
        });
    }

    return visitDto;
}
    public async Task<bool> ServiceExistsAsync(string serviceName, CancellationToken cancellationToken)
{
    await using var con = new SqlConnection(_connectionString);
    await using var cmd = new SqlCommand("SELECT COUNT(1) FROM service WHERE name = @name", con);
    cmd.Parameters.AddWithValue("@name", serviceName);
    await con.OpenAsync(cancellationToken);
    var count = (int)await cmd.ExecuteScalarAsync(cancellationToken);
    return count > 0;
}

public async Task InsertAppointmentAsync(
    int visitId, int clientId, int mechanicId,
    List<ServiceInputDto> services, CancellationToken cancellationToken)
{
    await using var con = new SqlConnection(_connectionString);
    await con.OpenAsync(cancellationToken);
    await using var tx = await con.BeginTransactionAsync(cancellationToken);

    try
    {
        await using var cmdVisit = new SqlCommand(
            "INSERT INTO visit (visit_id, client_id, mechanic_id, date) VALUES (@vid, @cid, @mid, @date)", con, (SqlTransaction)tx);
        cmdVisit.Parameters.AddWithValue("@vid", visitId);
        cmdVisit.Parameters.AddWithValue("@cid", clientId);
        cmdVisit.Parameters.AddWithValue("@mid", mechanicId);
        cmdVisit.Parameters.AddWithValue("@date", DateTime.Now);
        await cmdVisit.ExecuteNonQueryAsync(cancellationToken);

        foreach (var s in services)
        {
            await using var getServiceCmd = new SqlCommand("SELECT service_id FROM service WHERE name = @name", con, (SqlTransaction)tx);
            getServiceCmd.Parameters.AddWithValue("@name", s.ServiceName);
            var serviceId = (int?)await getServiceCmd.ExecuteScalarAsync(cancellationToken);

            if (serviceId == null)
                throw new Exception($"Service '{s.ServiceName}' not found during insert.");

            await using var cmdVs = new SqlCommand(
                "INSERT INTO visit_service (visit_id, service_id, service_fee) VALUES (@vid, @sid, @fee)", con, (SqlTransaction)tx);
            cmdVs.Parameters.AddWithValue("@vid", visitId);
            cmdVs.Parameters.AddWithValue("@sid", serviceId);
            cmdVs.Parameters.AddWithValue("@fee", s.ServiceFee);
            await cmdVs.ExecuteNonQueryAsync(cancellationToken);
        }

        await tx.CommitAsync(cancellationToken);
    }
    catch
    {
        await tx.RollbackAsync(cancellationToken);
        throw;
    }
}
public async Task<Mechanic?> GetMechanicByPwzAsync(string pwz, CancellationToken cancellationToken)
{
    await using var con = new SqlConnection(_connectionString);
    await using var cmd = new SqlCommand("SELECT * FROM mechanic WHERE licence_number = @pwz", con);
    cmd.Parameters.AddWithValue("@pwz", pwz);
    await con.OpenAsync(cancellationToken);

    var reader = await cmd.ExecuteReaderAsync(cancellationToken);
    if (await reader.ReadAsync())
    {
        return new Mechanic
        {
            mechanic_id = (int)reader["mechanic_id"],
            license_number = (string)reader["licence_number"],
            first_name = (string)reader["first_name"],
            last_name = (string)reader["last_name"]
        };
    }

    return null;
}


}