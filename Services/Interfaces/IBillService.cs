using QLKhachSanAPI.Models.Domains;
using QLKhachSanAPI.Models.DTOs;

namespace QLKhachSanAPI.Services.Interfaces
{
    public interface IBillService
    {
        Task<List<Bill>> GetAllBillsAsync();
        Task<List<Bill>> GetBillsByIDGuest(string id);
        
        Task<List<Bill>> GetBillsByStatus(bool status);
        Task<bool> CreateBillAsync(BillVM model);
        Task<bool> UpdateBillAsync(BillVM model);
        Task<bool> DeleteAllBillsAsync();
    }
}
