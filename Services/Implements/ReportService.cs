using QLKhachSanAPI.DataAccess;
using QLKhachSanAPI.Models.DAL;
using QLKhachSanAPI.Models.Domains;
using QLKhachSanAPI.Models.DTOs;
using QLKhachSanAPI.Services.Interfaces;

namespace QLKhachSanAPI.Services.Implements
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _dbContext;
        public ReportService(IUnitOfWork unitOfWork, AppDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }
        public async Task<bool> CreateReportAsync(Report model)
        {
            var report = await _unitOfWork.ReportRepository.GetSingleAsync(model.idGuest);

            if (report == null)
            {
                await _unitOfWork.ReportRepository.InsertAsync(model);
                await _unitOfWork.SaveEntitiesAsync();
            }
            else
            {
                await _unitOfWork.ReportRepository.DeleteAsync(model.idGuest);
                await _unitOfWork.SaveEntitiesAsync();
                await _unitOfWork.ReportRepository.InsertAsync(model);
                await _unitOfWork.SaveEntitiesAsync();
            }
            

            return true;
        }

        public async Task<bool> DeleteReportAsync(string id)
        {
            var report = await _unitOfWork.ReportRepository.GetSingleAsync(id);

            if (report == null)
            {
                throw new Exception("Report with provided id not found");
            }
            await _unitOfWork.ReportRepository.DeleteAsync(id);
            await _unitOfWork.SaveEntitiesAsync();

            return true;
        }

        public async Task<List<Report>> GetAllReportAsync()
        {
            return await _unitOfWork.ReportRepository.GetAsync();
        }
    }
}
