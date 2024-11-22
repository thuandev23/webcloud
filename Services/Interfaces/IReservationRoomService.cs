namespace QLKhachSanAPI.Services.Interfaces
{
    using Models.Domains;
    using Models.DTOs;
    

    public interface IReservationRoomService
    {
        Task<List<ReservationRoom>> GetAllReservationRoomsAsync();
        Task<ReservationRoom> GetReservationRoomsByRoomID(string ID);
        Task<List<ReservationRoom>> GetAllReservationRoomsByReservationID(string ID);
        Task<bool> CreateReservationRoomAsync(ReservationRoomVM model);
        
        Task<bool> DeleteAllReservationRoomsAsync();
    }
}
