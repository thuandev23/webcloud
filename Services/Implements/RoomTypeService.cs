namespace QLKhachSanAPI.Services.Implements
{
    using Microsoft.EntityFrameworkCore;
    using Models.DAL;
    using Models.Domains;
    using DataAccess;
    using Services.Interfaces;
    using Models.DTOs;
    using Extensions;

    public class RoomTypeService : IRoomTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _dbContext;   // for LinQ custom queries

        //private readonly IMemoryCache _memoryCache;
        //public string getAllRoomTypeCacheKey = "ListRoomTypes";

        public RoomTypeService(IUnitOfWork unitOfWork, AppDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        public async Task<List<RoomType>> GetAllRoomTypesAsync()
        {
            return await _unitOfWork.RoomTypeRepository.GetAsync();
        }

        public async Task<List<object>> GetAllRoomTypesLinQAsync()
        {
            var rooms = await _dbContext.RoomTypes
                .Select(r => new
                {
                    RoomTypeID = r.RoomTypeID,
                    Name = r.Name,
                    Description = r.Description,
                    AreaInSquareMeters = r.AreaInSquareMeters,
                    DateCreated = r.DateCreated,
                    DailyPrice = r.DailyPrice,
                    // Count the available rooms for this room type
                    AvailableRoomCount = r.Rooms!.Count(room => room.IsAvaiable == true) //
                }).ToListAsync();

            return rooms.Cast<object>().ToList();
        }


        public async Task<bool> CreateRoomTypeAsync(RoomTypeVM model)
        {
            var roomType = new RoomType
            {
                Name = model.Name,
                AreaInSquareMeters = model.AreaInSquareMeters,
                Description = model.Description,
                DailyPrice = model.DailyPrice,              
            };

            await _unitOfWork.RoomTypeRepository.InsertAsync(roomType);
            await _unitOfWork.SaveEntitiesAsync();
            return true;
        }


        public async Task<bool> UpdateRoomTypeAsync(RoomTypeVM model)
        {
            var roomType = await _unitOfWork.RoomTypeRepository.GetSingleAsync(model.IdToUpdate);

            if (roomType == null)
            {
                // Throw an exception or handle the case where the drink is not found
                throw new Exception("RoomType not found");
            }

            roomType.Name = model.Name;
            roomType.AreaInSquareMeters = model.AreaInSquareMeters;
            roomType.Description = model.Description;
           
            roomType.DailyPrice = model.DailyPrice;
            

            _unitOfWork.RoomTypeRepository.Update(roomType);
            await _unitOfWork.SaveEntitiesAsync();

            // Remove the cache entry for the modified data
            //_memoryCache.Remove(getAllDrinkTypeCacheKey);

            // Return true if the drink type was successfully added
            return true;
        }

        public async Task<bool> DeleteRoomTypeAsync(string roomTypeId)
        {
            var roomType = await _unitOfWork.RoomTypeRepository.GetSingleAsync(roomTypeId);

            if (roomType == null)
            {
                throw new Exception("RoomType with provided id not found");
            }
            await _unitOfWork.RoomTypeRepository.DeleteAsync(roomType);
            await _unitOfWork.SaveEntitiesAsync();

            // Remove the cache entry for the modified data
            //_memoryCache.Remove(getAllDrinkTypeCacheKey);

            return true;
        }

        public async Task<bool> DeleteAllRoomTypesAsync()
        {
            var roomTypes = await GetAllRoomTypesAsync();

            foreach (var roomType in roomTypes)
            {
                await _unitOfWork.RoomTypeRepository.DeleteAsync(roomType.RoomTypeID);
            }

            await _unitOfWork.SaveEntitiesAsync();

            // Remove the cache entry for the modified data
            //_memoryCache.Remove(getAllDrinkTypeCacheKey);

            return true;
        }

    }

}
