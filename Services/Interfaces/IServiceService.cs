using QLKhachSanAPI.Models.Domains;
using QLKhachSanAPI.Models.DTOs;

namespace QLKhachSanAPI.Services.Interfaces
{
    public interface IServiceService
    {
        Task<List<Service>> GetAllServiceAsync();
        Task<bool> CreateServiceAsync(ServiceVM model);
        Task<bool> UpdatServiceAsync(ServiceVM model);
        Task<bool> DeleteAllServiceAsync();
        Task<bool> DeleteServiceAsync(string IDService);
    }
}
