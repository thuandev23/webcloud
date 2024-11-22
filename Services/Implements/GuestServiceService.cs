using QLKhachSanAPI.Models.DTOs;
using QLKhachSanAPI.Services.Interfaces;
using QLKhachSanAPI.DataAccess;
using QLKhachSanAPI.Models.DAL;
using QLKhachSanAPI.Models.Domains;

namespace QLKhachSanAPI.Services.Implements
{
    public class GuestServiceService : IGuestServiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _dbContext;
        public GuestServiceService(IUnitOfWork unitOfWork, AppDbContext dbContext)
        {
            _unitOfWork=unitOfWork;
            _dbContext=dbContext;
        }
        public async Task<bool> CheckExitAsync(string IDService, string IDGuest)
        {
            Models.Domains.GuestService guestService = await _unitOfWork.GuestServiceRepository.GetSingleAsync(d => (d.ServiceID == IDService && d.GuestID == IDGuest));
            if (guestService == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> CreateGuestServiceAsync(GuestServiceVM model)
        {
            var guest = await _unitOfWork.GuestRepository.GetSingleAsync(model.GuestID);
            var service = await _unitOfWork.ServiceRepository.GetSingleAsync(model.ServiceID);
            if (guest != null && service != null)
            {
                var check = await CheckExitAsync( model.ServiceID,model.GuestID);
                if (check)
                {
                    var guestService = await _unitOfWork.GuestServiceRepository.GetSingleAsync(d => (d.ServiceID == model.ServiceID && d.GuestID == model.GuestID));
                    guestService.Number+= model.Number;
                    await _unitOfWork.SaveEntitiesAsync();

                    
                }
                else
                {
                    var guestService = new Models.Domains.GuestService
                    {
                        ServiceID = model.ServiceID,
                        GuestID = model.GuestID,
                        Number = model.Number
                    };
                    await _unitOfWork.GuestServiceRepository.InsertAsync(guestService);
                    await _unitOfWork.SaveEntitiesAsync();
                }

                //add bill
                var bill = await _unitOfWork.BillRepository.GetSingleAsync(d => (d.IDGuest == model.GuestID && d.Status == false));
                if (bill != null)
                {
                    bill.Sum = bill.Sum + service.Price * model.Number;
                    await _unitOfWork.SaveEntitiesAsync();
                }
                
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAllGuestServiceAsync()
        {
            var guestServices = await GetAllGuestServiceAsync();

            foreach (var guestService in guestServices)
            {
               _dbContext.Set<Models.Domains.GuestService>().Remove(guestService);
               
            }

            await _unitOfWork.SaveEntitiesAsync();

            // Remove the cache entry for the modified data
            //_memoryCache.Remove(getAllDrinkTypeCacheKey);

            return true;
        }

        public async Task<bool> DeleteGuestServiceAsync(string IDGuest)
        {
            var guestServices = await _unitOfWork.GuestServiceRepository.GetAsync(d => ( d.GuestID == IDGuest));
            foreach (var guestService in guestServices)
            {
                _dbContext.Set<Models.Domains.GuestService>().Remove(guestService);
                await _dbContext.SaveChangesAsync();
            }
            return true;
        }

        public async Task<List<Models.Domains.GuestService>> GetAllGuestServiceAsync()
        {
            return await _unitOfWork.GuestServiceRepository.GetAsync();
        }

        public async Task<int> GetNumberAsync(string IDService, string IDGuest)
        {
            Models.Domains.GuestService guestService = await _unitOfWork.GuestServiceRepository.GetSingleAsync(d=>(d.ServiceID==IDService&&d.GuestID==IDGuest));
            if (guestService == null)
            {
                return -1;
            }
            return guestService.Number;
        }

        
    }
}
