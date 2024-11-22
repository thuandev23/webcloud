using QLKhachSanAPI.Models.Domains;
using QLKhachSanAPI.Models.DTOs;

namespace QLKhachSanAPI.Services.Interfaces
{
    public interface IReservationService
    {
        Task<List<Reservation>> GetAllReservationsAsync();
        Task<List<object>> GetAllReservationsAsyncLinQ();
        Task<Reservation> GetReservationsByID(string Id);
        Task<List<Reservation>> GetReservationsByGuestID(string Id);

        Task<List<Reservation>> GetReservationsByWasConfirm(bool isConfirm);// lấy những phiếu phòng đã xác nhận hay chưa
        Task<List<object>> GetReservationsByWasConfirmLinQ(bool isConfirm);
        Task<List<Reservation>> GetReservationsByDate(DateTime StartTime, DateTime EndTime); //lấy những phiếu trong range của 2 ngày trên
       /* Task<bool> ConfirmReservationAsync(ConfirmReservationVM model);*/
        Task<bool> ReserveRoomsAsync(ReservationViewModel reservationvm);
        Task<bool> CreateReservationAsync(ReservationVM model);
        Task<bool> UpdateReservationAsync(ReservationVM model);
        Task<bool> DeleteAllReservationsAsync();
        Task<bool> CheckIn(string IDReservation);
        Task<bool> CheckOut(string IDReservation);
        Task<bool> Cancel(string IDReservation);
        Task<Reservation> GetReservationByRoom(string IDRoom);

    }
}
