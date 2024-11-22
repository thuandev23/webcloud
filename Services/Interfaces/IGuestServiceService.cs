using QLKhachSanAPI.Models.Domains;
using QLKhachSanAPI.Models.DTOs;

namespace QLKhachSanAPI.Services.Interfaces
{
    public interface IGuestServiceService
    {
        Task<List<GuestService>> GetAllGuestServiceAsync();
        Task<int> GetNumberAsync(string IDService, string IDGuest);
        Task<bool> CheckExitAsync(string IDService, string IDGuest);
        Task<bool> CreateGuestServiceAsync(GuestServiceVM model);
        Task<bool> DeleteAllGuestServiceAsync();
        Task<bool> DeleteGuestServiceAsync(string IDGuest);

    }
}
