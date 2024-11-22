using QLKhachSanAPI.DataAccess;
using QLKhachSanAPI.Models.Domains;
using QLKhachSanAPI.Models.DTOs;
using QLKhachSanAPI.Services.Interfaces;

namespace QLKhachSanAPI.Services.Implements
{
    public class ReservationRoomService : IReservationRoomService
    {

        private readonly IUnitOfWork _unitOfWork;

        //private readonly IMemoryCache _memoryCache;
        //public string getAllRoomTypeCacheKey = "ListRoomTypes";

        public ReservationRoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateReservationRoomAsync(ReservationRoomVM model)
        {
            var reservationID = await _unitOfWork.ReservationRepository.GetSingleAsync(model.ReservationID);
            var roomID = await _unitOfWork.RoomRepository.GetSingleAsync(model.RoomID);
            if (reservationID != null && roomID != null)
            {
                var reservationRoom = new ReservationRoom
                {
                    ReservationID = model.ReservationID,
                    RoomID = model.RoomID,
                };
                await _unitOfWork.ReservationRoomRepository.InsertAsync(reservationRoom);
                await _unitOfWork.SaveEntitiesAsync();
                return true;
            }
            return false;
            
        }

        public async Task<bool> DeleteAllReservationRoomsAsync()
        {
            var reservationRooms = await GetAllReservationRoomsAsync();

            foreach (var reservationRoom in reservationRooms)
            {
                await _unitOfWork.ReservationRoomRepository.DeleteAsync(reservationRoom.ReservationID);
                await _unitOfWork.ReservationRoomRepository.DeleteAsync(reservationRoom.RoomID);
            }

            await _unitOfWork.SaveEntitiesAsync();

            // Remove the cache entry for the modified data
            //_memoryCache.Remove(getAllDrinkTypeCacheKey);

            return true;
        }

        public async Task<List<ReservationRoom>> GetAllReservationRoomsAsync()
        {
            return await _unitOfWork.ReservationRoomRepository.GetAsync();
        }

        public async Task<List<ReservationRoom>> GetAllReservationRoomsByReservationID(string ID)
        {
            return await _unitOfWork.ReservationRoomRepository.GetAsync(d => d.ReservationID == ID);
        }

        public async Task<ReservationRoom> GetReservationRoomsByRoomID(string ID)
        {
            return await _unitOfWork.ReservationRoomRepository.GetSingleAsync(d => d.RoomID == ID);
        }

    



        
    }
}
