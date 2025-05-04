using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientService : IClientService
{
    private readonly string _connectionString = "Data Source=localhost, 1433; User=SA; Password=yourStrong(!)Password; Initial Catalog=APBD; Integrated Security=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False";

    public async Task<bool> DoesClientExist(int id)
    {
        string command = "SELECT 1 FROM Client WHERE IdClient = @id"; // zwraca 1 jesli istnieje w bazie klient o danym id
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@id", id);
            
            await conn.OpenAsync();
            
            var res = await cmd.ExecuteScalarAsync();
            return res != null;
        }
    }

    public async Task<int> AddClient(ClientDTO client)
    {
        // wstawienie danych klienta do tabeli Client
        string command =
            @"INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel) VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel); SELECT SCOPE_IDENTITY();";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@FirstName", client.FirstName);
            cmd.Parameters.AddWithValue("@LastName", client.LastName);
            cmd.Parameters.AddWithValue("@Email", client.Email);
            cmd.Parameters.AddWithValue("@Telephone", client.Telephone);
            cmd.Parameters.AddWithValue("@Pesel", client.Pesel);
            
            await conn.OpenAsync();
            var res = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(res);
        }
    }

    public async Task<bool> RegisterClientForTrip(int clientId, int tripId)
    {
        // wstawiam rejestracje klienta o danym id na wycieczke o danym id do tabeli Client_Trip wraz z data wstawienia (dzisiejszą)
        string command =
            "INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt) VALUES (@clientId, @tripId, @registeredAt);";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@clientId", clientId);
            cmd.Parameters.AddWithValue("@tripId", tripId);
            cmd.Parameters.AddWithValue("@registeredAt", Convert.ToInt32(DateTime.UtcNow.ToString("yyyyMMdd")));
            
            await conn.OpenAsync();
            
            var result = await cmd.ExecuteNonQueryAsync();
            return result > 0;
        }
    }

    public async Task<bool> IsClientRegisteredForTrip(int clientId, int tripId)
    {
        // zwraca 1 jesli istnieje rejestracja klienta o danym id na wycieczke o danym id w tabeli Client_Trip
        string command = "SELECT 1 FROM Client_Trip WHERE IdClient = @idClient AND IdTrip = @idTrip";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@idClient", clientId);
            cmd.Parameters.AddWithValue("@idTrip", tripId);
            
            await conn.OpenAsync();
            
            var res = await cmd.ExecuteScalarAsync();
            return res != null;
        }
    }

    public async Task<bool> DeleteRegistration(int clientId, int tripId)
    {
        // usuwa rejestracje klienta na dana wycieczke z tabeli Client_Trip
        string command = @"DELETE FROM Client_Trip WHERE IdClient = @idClient AND IdTrip = @idTrip";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@idClient", clientId);
            cmd.Parameters.AddWithValue("@idTrip", tripId);
            
            await conn.OpenAsync();
            
            var result = await cmd.ExecuteNonQueryAsync();
            return result > 0;
        }
    }
}