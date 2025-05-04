using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface ITripsService
{
    // zwraca informacje o wszystkich wycieczkach (id, nazwa, opis, dataPoczatkowa, dataKoncowa, max liczba osob, kraje)
    Task<List<TripDTO>> GetTrips();
    
    // zwraca dane wycieczek klienta o danym id (id klienta, id wycieczki, nazwa wycieczki, opis, dataPoczatkowa, dataKoncowa, max liczba osob, kiedy zarejestrowana, kiedy oplacone)
    Task<List<ClientTripDTO>> GetClientTrips(int idClient);
    
    // sprawdza czy w bazie znajduje sie wycieczka o danym id
    Task<bool> DoesTripExist(int id);
}