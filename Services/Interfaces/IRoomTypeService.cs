namespace QLKhachSanAPI.Services.Interfaces
{
    using Models.Domains;
    using Models.DTOs;

    public interface IRoomTypeService
    {
        Task<List<RoomType>> GetAllRoomTypesAsync();
        Task<List<object>> GetAllRoomTypesLinQAsync();
        Task<bool> CreateRoomTypeAsync(RoomTypeVM model);
        Task<bool> UpdateRoomTypeAsync(RoomTypeVM model);
        Task<bool> DeleteAllRoomTypesAsync();
    }
}
