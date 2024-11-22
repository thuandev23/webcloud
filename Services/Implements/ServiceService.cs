using Microsoft.EntityFrameworkCore;
using QLKhachSanAPI.DataAccess;
using QLKhachSanAPI.Models.DAL;
using QLKhachSanAPI.Models.Domains;
using QLKhachSanAPI.Models.DTOs;
using QLKhachSanAPI.Services.Interfaces;

namespace QLKhachSanAPI.Services.Implements
{
    public class ServiceService : IServiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _dbContext;
        public ServiceService(IUnitOfWork unitOfWork, AppDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }
        public async Task<bool> CreateServiceAsync(ServiceVM model)
        {
            var service = new Service
            {
                Name = model.Name,
                Price =model.Price,
                Description =model.Description
            };

            await _unitOfWork.ServiceRepository.InsertAsync(service);
            await _unitOfWork.SaveEntitiesAsync();
            return true;
        }

        public async Task<bool> DeleteServiceAsync(string IDService)
        {
            var service = await _unitOfWork.ServiceRepository.GetSingleAsync(IDService);

            if (service == null)
            {
                throw new Exception("Service with provided id not found");
            }
            await _unitOfWork.ServiceRepository.DeleteAsync(service.ID);
            await _unitOfWork.SaveEntitiesAsync();

            // Remove the cache entry for the modified data
            //_memoryCache.Remove(getAllDrinkTypeCacheKey);

            return true;
        }

        public async Task<bool> DeleteAllServiceAsync()
        {
            var services = await GetAllServiceAsync();

            foreach (var service in services)
            {
                await _unitOfWork.ServiceRepository.DeleteAsync(service.ID);
            }

            await _unitOfWork.SaveEntitiesAsync();

            // Remove the cache entry for the modified data
            //_memoryCache.Remove(getAllDrinkTypeCacheKey);

            return true;
        }

        public async Task<List<Service>> GetAllServiceAsync()
        {
            return await _unitOfWork.ServiceRepository.GetAsync();
        }

        public async Task<bool> UpdatServiceAsync(ServiceVM model)
        {
            var service = await _unitOfWork.ServiceRepository.GetSingleAsync(model.IdToUpdate);

            if (service == null)
            {
                // Throw an exception or handle the case where the drink is not found
                throw new Exception("Service not found");
            }

            service.Name = model.Name;
            service.Description = model.Description;
            service.Price = model.Price;

            _unitOfWork.ServiceRepository.Update(service);
            await _unitOfWork.SaveEntitiesAsync();

            // Remove the cache entry for the modified data
            //_memoryCache.Remove(getAllDrinkTypeCacheKey);

            // Return true if the drink type was successfully added
            return true;
        }
    }
}
