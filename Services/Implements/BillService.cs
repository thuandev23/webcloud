using QLKhachSanAPI.DataAccess;
using QLKhachSanAPI.Models.Domains;
using QLKhachSanAPI.Models.DTOs;
using QLKhachSanAPI.Services.Interfaces;

namespace QLKhachSanAPI.Services.Implements
{
    public class BillService : IBillService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BillService(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }

        public async Task<bool> CreateBillAsync(BillVM model)
        {
           
            var guest = await _unitOfWork.GuestRepository.GetSingleAsync(model.IDGuest);
            if (guest == null)
            {
                return false;
            }
            Bill b =new Bill();
            b.Sum = model.Sum;
            b.Status = model.Status;
           
            b.IDGuest = model.IDGuest;

            await _unitOfWork.BillRepository.InsertAsync(b);
            await _unitOfWork.SaveEntitiesAsync();
            return true;
        }

        public async Task<bool> DeleteAllBillsAsync()
        {
            var bills = await GetAllBillsAsync();

            foreach (var bill in bills)
            {
                await _unitOfWork.BillRepository.DeleteAsync(bill.ID);
            }

            await _unitOfWork.SaveEntitiesAsync();

            // Remove the cache entry for the modified data
            //_memoryCache.Remove(getAllDrinkTypeCacheKey);

            return true;
        }

        public async Task<List<Bill>> GetAllBillsAsync()
        {
            return await _unitOfWork.BillRepository.GetAsync();
        }

        public async Task<List<Bill>> GetBillsByIDGuest(string id)
        {
            return await _unitOfWork.BillRepository.GetAsync(d=>d.IDGuest==id);
        }

        

        public async Task<List<Bill>> GetBillsByStatus(bool status)
        {
            return await _unitOfWork.BillRepository.GetAsync(d => d.Status == status);
        }

        public async Task<bool> UpdateBillAsync(BillVM model)
        {
            var bill = await _unitOfWork.BillRepository.GetSingleAsync(model.IdToUpdate);

            if (bill == null)
            {
                // Throw an exception or handle the case where the drink is not found
                throw new Exception("Bill not found");
            }

           
            var guest = await _unitOfWork.GuestRepository.GetSingleAsync(model.IDGuest);
            if( guest == null)
            {
                return false;
            }

            bill.Sum=model.Sum;
            bill.Status=model.Status;
          
            bill.IDGuest=model.IDGuest;

            _unitOfWork.BillRepository.Update(bill);
            await _unitOfWork.SaveEntitiesAsync();

            // Remove the cache entry for the modified data
            //_memoryCache.Remove(getAllDrinkTypeCacheKey);

            // Return true if the drink type was successfully added
            return true;
        }
    }
}
