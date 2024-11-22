using QLKhachSanAPI.DataAccess;
using QLKhachSanAPI.Models.Domains;
using QLKhachSanAPI.Models.DTOs;
using QLKhachSanAPI.Services.Interfaces;

namespace QLKhachSanAPI.Services.Implements
{
    public class GuestService : IGuestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReservationService _reservationService;
        public GuestService(IUnitOfWork unitOfWork, IReservationService reservationService)
        {
            _unitOfWork = unitOfWork;
            _reservationService = reservationService;
        }
        public async Task<bool> CreateGuestAsync(GuestVM model)
        {
            Guest g = new Guest();
            g.FullName = model.FullName;
          
            g.PhoneNumber = model.PhoneNumber;
            g.Age = model.Age;
            g.Email = model.Email;
            await _unitOfWork.GuestRepository.InsertAsync(g);
            await _unitOfWork.SaveEntitiesAsync();
            return true;
        }

        public async Task<bool> DeleteAllGuestsAsync()
        {
            var guests = await GetAllGuestsAsync();

            foreach (var guest in guests)
            {
                await _unitOfWork.GuestRepository.DeleteAsync(guest.GuestID);
            }

            await _unitOfWork.SaveEntitiesAsync();

            // Remove the cache entry for the modified data
            //_memoryCache.Remove(getAllDrinkTypeCacheKey);

            return true;
        }

        public async Task<List<Guest>> GetAllGuestsAsync()
        {
            return await _unitOfWork.GuestRepository.GetAsync();
        }

        public async Task<List<Guest>> GetGuestsByName(string fullName)
        {
            return await _unitOfWork.GuestRepository.GetAsync(g => g.FullName == fullName);
        }

        public async Task<Guest> GetGuestsByEmail(string Email)
        {
            return await _unitOfWork.GuestRepository.GetSingleAsync(g => g.Email == Email);
        }

        public async Task<bool> UpdateGuestAsync(GuestVM model)
        {
            var guest = await _unitOfWork.GuestRepository.GetSingleAsync(model.IdToUpdate);

            if (guest == null)
            {
                // Throw an exception or handle the case where the drink is not found
                throw new Exception("Guest not found");
            }

            guest.PhoneNumber = model.PhoneNumber;
            guest.Email = model.Email;
            guest.FullName = model.FullName;
            guest.Age = model.Age;
          

            _unitOfWork.GuestRepository.Update(guest);
            await _unitOfWork.SaveEntitiesAsync();

            // Remove the cache entry for the modified data
            //_memoryCache.Remove(getAllDrinkTypeCacheKey);

            // Return true if the drink type was successfully added
            return true;
        }

        public async Task<Guest> GetGuestByRoom(string IDRoom)
        {
            Reservation reservation= await _reservationService.GetReservationByRoom(IDRoom);
            
            return await _unitOfWork.GuestRepository.GetSingleAsync(reservation?.GuestID);
        }
    }
}
