namespace QLKhachSanAPI.Services.Interfaces
{
    using Models.Domains;
    using Models.DTOs;
    public interface IReportService
    {
        Task<List<Report>> GetAllReportAsync();
        Task<bool> CreateReportAsync(Report model);
        Task<bool> DeleteReportAsync(string id);
    }
}
