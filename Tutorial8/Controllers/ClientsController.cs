using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly ITripsService _tripsService;
    private readonly IClientService _clientService;
    
    public ClientsController(ITripsService tripsService, IClientService clientService)
    {
        _tripsService = tripsService;
        _clientService = clientService;
    }
    
    // zwraca wszystkie wycieczki zwiazane z danym klientem
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetTripsOfClient(int id)
    {
        if (! await _clientService.DoesClientExist(id))
        {
            return NotFound("Taki klient nie istnieje.");
        }

        var trips = await _tripsService.GetClientTrips(id);
        if (trips.Count == 0) return NotFound("Ten klient nie ma wycieczek.");
        return Ok(trips);
    }

    // tworzy nowy rekord klienta z danymi (FirstName, LastName, Email, Telephone, Pesel)
    [HttpPost]
    public async Task<IActionResult> AddClient(ClientDTO client)
    {
        var id = await _clientService.AddClient(client);
        return Ok(id);
    }

    // rejestruje klienta o danym id na wycieczke o danym id
    [HttpPost("{id}/trips/{tripId}")]
    public async Task<IActionResult> AddClientTrip(int id, int tripId)
    {
        // sprawdzenie czy klient o danym id istnieje
        if (! await _clientService.DoesClientExist(id))
        {
            return NotFound("Taki klient nie istnieje.");
        }

        // sprawdzenie czy wycieczka o danym id istnieje
        if (!await _tripsService.DoesTripExist(tripId))
        {
            return NotFound("Taka wycieczka nie istnieje.");
        }
        
        var res = await _clientService.RegisterClientForTrip(id, tripId);
        if (Convert.ToBoolean(res)) return Created();
        return StatusCode(500, "Błąd dodania.");
    }

    // usuniecie rejestracji klienta o danym id na wycieczke o danym id
    [HttpDelete("{id}/trips/{tripId}")]
    public async Task<IActionResult> DeleteClientTrip(int id, int tripId)
    {
        // sprawdzenie czy klient jest zarejestrowany na dana wycieczke
        if (!await _clientService.IsClientRegisteredForTrip(id, tripId))
        {
            return NotFound("Takiej rejestracji nie ma.");
        }
        
        var res = await _clientService.DeleteRegistration(id, tripId);
        if (Convert.ToBoolean(res)) return Ok("Poprawnie usunięto.");
        return StatusCode(500, "Błąd usunięcia.");
    }
}