namespace QLKhachSanAPI.Services.Implements
{
    using DataAccess;
    using Microsoft.AspNetCore.Http.HttpResults;
    using Microsoft.EntityFrameworkCore;
    using Models.DAL;
    using Models.Domains;
    using Models.DTOs;
    
    using Extensions;
    using Services.Interfaces;
    using System.Runtime.ExceptionServices;

    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _dbContext;
        

        // for LinQ custom queries

        //private readonly IMemoryCache _memoryCache;
        //public string getAllRoomTypeCacheKey = "ListRoomTypes";

        public RoomService(IUnitOfWork unitOfWork, AppDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
           
        }

        public async Task<List<Room>> GetAllRoomsAsync()
        {
            return await _unitOfWork.RoomRepository.GetAsync();
        }

        public async Task<List<object>> GetAllRoomsLinQAsync()
        {
            //return await _unitOfWork.RoomRepository.GetAsync();
            var rooms = await _dbContext.Rooms
                .Include(r => r.RoomType)
                .Select(r => new
                {
                    Id = r.RoomID,
                    RoomNumber = r.RoomNumber,
                    RoomType = new
                    {
                        Name = r.RoomType.Name,
                        // Capacity = r.RoomType.Capacity,
                        RoomTypeId = r.RoomType.RoomTypeID,
                        Description = r.RoomType.Description,
                        AreaInSquareMeters = r.RoomType.AreaInSquareMeters,
                        DailyPrice = r.RoomType.DailyPrice,
                    },      
                    DateCreated = r.DateCreated,
                    IsAvailable = r.IsAvaiable,                      
                }).ToListAsync();
            return rooms.Cast<object>().ToList();
        }

        public async Task<Room> GetRoomsById(string roomId)
        {
            var room = await _unitOfWork.RoomRepository.GetSingleAsync(d => d.RoomID == roomId);
            return room;
        }

        // tìm phòng theo loại phòng
        public async Task<List<Room>> GetRoomsByTypeName(string roomTypeName)
        {
            var rooms = await _unitOfWork.RoomRepository.GetAsync(d => d.RoomType.Name == roomTypeName);
            return rooms;
        }
        public async Task<List<Room>> GetRoomsByTypeId(string roomTypeId)
        {
            var rooms = await _unitOfWork.RoomRepository.GetAsync(d => d.RoomType.RoomTypeID == roomTypeId);
            return rooms;
        }

        //dalse is room have exists
        public async Task<bool> CheckRoomByNumberRoom(string numberRoom)
        {
            var room = await  _unitOfWork.RoomRepository.GetSingleAsync(d => d.RoomNumber == numberRoom);
            if (room!=null) return false;

            return true;
        }

        public async Task<bool> CreateRoomAsync(RoomVM model)
        {
            var check = await CheckRoomByNumberRoom(model.RoomNumber);

            if(!check){
                throw new Exception("Room number already exists");
               
            }
            var roomTypeID = await _unitOfWork.RoomTypeRepository.GetSingleAsync(model.RoomTypeID);
            if (roomTypeID != null)
            {
                var room = new Room
                {

                    RoomNumber = model.RoomNumber,
                    RoomTypeID = model.RoomTypeID,
                    IsAvaiable = true,

                };
                await _unitOfWork.RoomRepository.InsertAsync(room);
                await _unitOfWork.SaveEntitiesAsync();
                return true;
            }
            return false;

        }


        public async Task<bool> UpdateRoomAsync(RoomVM model)
        {
            var check = await CheckRoomByNumberRoom(model.RoomNumber);

            if (!check)
            {
                throw new Exception("Room number already exists");

            }
            var room = await _unitOfWork.RoomRepository.GetSingleAsync(model.IdToUpdate);

            if (room == null)
            {
                // Throw an exception or handle the case where the drink is not found
                throw new Exception("Room not found");
            }
            var roomTypeID = await _unitOfWork.RoomTypeRepository.GetSingleAsync(model.RoomTypeID);
            if (roomTypeID == null)
            {
                return false;
            }
            room.RoomNumber = model.RoomNumber;
            room.RoomTypeID = model.RoomTypeID;
            room.IsAvaiable = model.IsAvaiable;

            _unitOfWork.RoomRepository.Update(room);
            await _unitOfWork.SaveEntitiesAsync();

            // Remove the cache entry for the modified data
            //_memoryCache.Remove(getAllDrinkTypeCacheKey);

            // Return true if the drink type was successfully added
            return true;
        }



        public async Task<bool> DeleteAllRoomsAsync()
        {
            var rooms = await GetAllRoomsAsync();

            foreach (var room in rooms)
            {
                await _unitOfWork.RoomTypeRepository.DeleteAsync(room.RoomID);
            }

            await _unitOfWork.SaveEntitiesAsync();

            // Remove the cache entry for the modified data
            //_memoryCache.Remove(getAllDrinkTypeCacheKey);

            return true;
        }

        public async Task<List<Room>> GetRoomsByIsAvailable(bool IsAvailable)
        {
            return await _unitOfWork.RoomRepository.GetAsync(d => d.IsAvaiable == IsAvailable);
        }


        public async Task<List<Room>> GetRoomIsReser(DateTime StartTime, DateTime EndTime, RoomType RoomType)
        {
            var reservations =  await _unitOfWork.ReservationRepository.GetAsync(d => ((DateTime.Compare(StartTime, d.StartTime) > 0 && DateTime.Compare(StartTime, d.EndTime) < 0) || (DateTime.Compare(EndTime, d.StartTime) > 0 && DateTime.Compare(EndTime, d.EndTime) < 0) || (DateTime.Compare(d.StartTime, StartTime) >= 0 && DateTime.Compare(d.EndTime, EndTime) <= 0)));
     
            List<Room> rooms = new List<Room>();
            foreach(var reservation in reservations)
            {
                var reservationRooms =  await _unitOfWork.ReservationRoomRepository.GetAsync(d => d.ReservationID == reservation.ReservationID);
                foreach (var reservationRoom in reservationRooms)
                {
                    Room r = await GetRoomsById(reservationRoom.RoomID);
                    if (r.RoomTypeID == RoomType.RoomTypeID)
                    {
                        rooms.Add(r);
                    }
                }
            }
            return rooms;
        }

        public async Task<List<Room>> GetRoomNotReser(DateTime StartTime, DateTime EndTime, RoomType RoomType)
        {

            List<Room> rooms = await GetRoomsByTypeId(RoomType.RoomTypeID);
            List<Room> roomIsResers = await GetRoomIsReser(StartTime, EndTime,RoomType);
            List<Room> roomNotResers = rooms.Except(roomIsResers).ToList();
            return roomNotResers;
        }

        public async Task<bool> CheckRoom(DateTime StartTime, DateTime EndTime, RoomType RoomType,int NumberRoomWantReser)
        {
            var roomNotResers = await GetRoomNotReser(StartTime, EndTime, RoomType);
            if (NumberRoomWantReser > roomNotResers.Count)
            {
                return false;
            }
            return true;
        }

        
       
    }
}
