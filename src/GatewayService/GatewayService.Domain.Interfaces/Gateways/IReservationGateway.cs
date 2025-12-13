using GatewayService.Domain.Models.Reservations;

namespace GatewayService.Domain.Interfaces.Gateways;

public interface IReservationGateway
{
    public Task<int> GetCurrentReservationsCountByUsernameAsync(string username, string accessToken);

    public Task<ReservationShort> AddReservationAsync(string username, ReservationCreate reservationCreate, string accessToken);
    
    public Task<List<ReservationShort>> GetReservationsByUsernameAsync(string username, string accessToken);

    public Task<ReservationShort?> DeleteReservationAsync(Guid reservationId, DateOnly date, string accessToken);
    
    public Task DeleteReservationAsync(Guid reservationId, string accessToken);
}