using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=localhost, 1433; User=SA; Password=yourStrong(!)Password; Initial Catalog=APBD; Integrated Security=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False";
    
    public async Task<List<TripDTO>> GetTrips()
    {
        // pobieram wszystkie informacje o wycieczkach
        var trips = new List<TripDTO>();
        var inds = new List<int>();  // id wycieczek juz stworzonych jako obiekty

        // zwraca dane wycieczki wraz z krajami w ktorym sie odbywa (po jednym kraju dla kazdego rekordu, jedna wycieczka moze byc zapisana w kilku rekordach, jesli zawiera kilka krajow)
        string command = "SELECT T.IdTrip, T.Name, T.Description, T.DateFrom, T.DateTo, T.MaxPeople, C.Name " +
                         "FROM Trip T " +
                         "JOIN Country_Trip CT ON CT.IdTrip = T.IdTrip " +
                         "JOIN Country C ON C.IdCountry = CT.IdCountry";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdTrip");
                    int id = reader.GetInt32(idOrdinal);
                    if (inds.Contains(id)) // jezeli wycieczka o tym id byla juz stworzona to dodaje do jej listy krajow kolejny kraj
                    {
                        foreach (var trip in trips)
                        {
                            if (trip.Id == id)
                            {
                                trip.Countries.Add(new CountryDTO(){Name = reader.GetString(6)});
                            }
                        }
                    }
                    else
                    {
                        trips.Add(new TripDTO() // tworze nowy obiekt wycieczki
                        {
                            Id = reader.GetInt32(idOrdinal),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            DateFrom = reader.GetDateTime(3),
                            DateTo = reader.GetDateTime(4),
                            MaxPeople = reader.GetInt32(5),
                            Countries = new List<CountryDTO> { new CountryDTO() { Name = reader.GetString(6) } }
                        });
                        inds.Add(id); // wycieczka o tym id juz stworzona
                    }
                }
            }
        }
        return trips;
    }

    public async Task<List<ClientTripDTO>> GetClientTrips(int idClient)
    {
        var trips = new List<ClientTripDTO>();
        
        // zwracam informacje o wycieczkach z tabeli Trip klienta o danym id
        string command = "SELECT T.IdTrip, T.Name, T.Description, T.DateFrom, T.DateTo, T.MaxPeople, CT.RegisteredAt, CT.PaymentDate " +
                         "FROM Trip T " +
                         "JOIN Client_Trip CT ON T.IdTrip = CT.IdTrip " +
                         "JOIN Client C ON C.IdClient = CT.IdClient " +
                         "WHERE C.IdClient = @id";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@id", idClient);

            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    trips.Add(new ClientTripDTO()
                    {
                        IdClient = idClient,
                        IdTrip = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2),
                        DateFrom = reader.GetDateTime(3),
                        DateTo = reader.GetDateTime(4),
                        MaxPeople = reader.GetInt32(5),
                        RegisteredAt = reader.GetInt32(6),
                        PaymentDate = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7)
                    });
                }
            }
        }
        return trips;
    }

    public async Task<bool> DoesTripExist(int idTrip)
    {
        // zwrac 1 jeśli istnieje wycieczka o podanym id w tabeli Trip
        string query = @"SELECT 1 FROM Trip Where IdTrip = @idTrip";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@idTrip", idTrip);
            
            await conn.OpenAsync();
            
            var result = await cmd.ExecuteReaderAsync();
            return Convert.ToBoolean(result.Read());
        }
    }
}