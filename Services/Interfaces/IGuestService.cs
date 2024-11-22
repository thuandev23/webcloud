using QLKhachSanAPI.Models.Domains;
using QLKhachSanAPI.Models.DTOs;

namespace QLKhachSanAPI.Services.Interfaces
{
    public interface IGuestService
    {
        Task<List<Guest>> GetAllGuestsAsync();
        Task<List<Guest>> GetGuestsByName(string name);
        Task<bool> CreateGuestAsync(GuestVM model);
        Task<bool> UpdateGuestAsync(GuestVM model);
        Task<bool> DeleteAllGuestsAsync();
        Task<Guest> GetGuestByRoom(string IDRoom);
    }
}
