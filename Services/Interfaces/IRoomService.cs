namespace QLKhachSanAPI.Services.Interfaces
{
    using Models.Domains;
    using Models.DTOs;

    public interface IRoomService
    {
        Task<List<Room>> GetAllRoomsAsync();
        Task<List<object>> GetAllRoomsLinQAsync();
        Task<Room> GetRoomsById(string roomId);
        Task<List<Room>> GetRoomsByTypeName(string roomTypeName);
        Task<List<Room>> GetRoomsByTypeId(string roomTypeId);
        Task<List<Room>> GetRoomsByIsAvailable(bool status); //lấy phòng còn trống
        Task<bool> CheckRoomByNumberRoom(string roomNumber);
        Task<bool> CreateRoomAsync(RoomVM model);
        Task<bool> UpdateRoomAsync(RoomVM model);
        Task<bool> DeleteAllRoomsAsync();
        Task<List<Room>> GetRoomIsReser(DateTime StartTime, DateTime EndTime, RoomType RoomType); // lấy phòng đã đặt tùy loại phòng
        Task<List<Room>> GetRoomNotReser(DateTime StartTime, DateTime EndTime, RoomType RoomType); // lấy phòng chưa đặt tùy loại phòng
        Task<bool> CheckRoom(DateTime StartTime, DateTime EndTime, RoomType RoomType,int NumberRoomWantReser); //kiểm tra phòng còn số lượng phòng trong khoảng time trên tùy vào loại phòng. True là còn phòng

    }
}
