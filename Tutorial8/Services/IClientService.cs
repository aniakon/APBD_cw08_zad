using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientService
{
    // sprawdzenie czy klient o danym id jest w bazie
    Task<bool> DoesClientExist(int idClient);
    
    // dodanie klienta do bazy (FirstName, LastName, Email, Telephone, Pesel)
    Task<int> AddClient(ClientDTO client);
    
    // rejestruje klienta o danym id na wycieczke o danym id
    Task<bool> RegisterClientForTrip(int clientId, int tripId);
    
    // sprawdza czy klient jest zarejestrowany na wycieczke
    Task<bool> IsClientRegisteredForTrip(int clientId, int tripId);
    
    // usuwa rejestracje klienta na wycieczke
    Task<bool> DeleteRegistration(int clientId, int tripId);
}